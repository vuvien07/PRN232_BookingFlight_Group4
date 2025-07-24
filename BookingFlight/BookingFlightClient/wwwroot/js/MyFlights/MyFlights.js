// My Flights JavaScript - Main functionality for the flight management page

// Test basic functionality
console.log('🚀 MyFlights.js loaded successfully');

// Test global function availability
console.log('🔍 Global getAuthToken available:', typeof window.getAuthToken === 'function');

// Define global getAuthToken function if not available
if (typeof window.getAuthToken !== 'function') {
    console.log('🔧 Defining global getAuthToken function');
    window.getAuthToken = function() {
        console.log('🌐 Global getAuthToken called');
        
        // Helper function to get cookie
        const getCookie = (name) => {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
            return null;
        };
        
        // Try to get JWT token from cookies first, then localStorage, sessionStorage
        const cookieToken = getCookie('X-Access-Token');
        const localStorageAuthToken = localStorage.getItem('authToken');
        const sessionStorageAuthToken = sessionStorage.getItem('authToken');
        const localStorageToken = localStorage.getItem('token');
        const sessionStorageToken = sessionStorage.getItem('token');
        const flightInfoToken = localStorage.getItem('flightInfoToken');
        const flightCheckoutToken = localStorage.getItem('flightCheckoutToken');
        const accessToken = localStorage.getItem('access_token') || sessionStorage.getItem('access_token');
        const altCookieToken = getCookie('authToken') || getCookie('token') || getCookie('jwt');
        
        const finalToken = cookieToken || 
                          localStorageAuthToken || 
                          sessionStorageAuthToken || 
                          localStorageToken || 
                          sessionStorageToken ||
                          accessToken ||
                          altCookieToken || '';
        
        // DO NOT include flight-related tokens like flightInfoToken, flightCheckoutToken
        
        console.log('🌐 Global getAuthToken result:', finalToken ? finalToken.substring(0, 30) + '...' : 'NO TOKEN');
        return finalToken;
    };
    console.log('✅ Global getAuthToken function defined');
}

class MyFlightsManager {
    constructor() {
        // Detect current host and adjust API URL accordingly
        const currentHost = window.location.hostname;
        const currentPort = window.location.port;
        console.log('🌐 Current host:port:', currentHost + ':' + currentPort);
        
        // SOLUTION: Try client proxy first (since user is logged in), then direct server
        this.apiUrls = [
            `/MyFlights/api`,  // Primary: Through client proxy (user is authenticated here)
            `http://${currentHost}:5077/api`   // Fallback: HTTP API server direct
        ];
        this.currentApiIndex = 0;
        this.apiBaseUrl = this.apiUrls[0];
        console.log('🔗 Primary API URL:', this.apiBaseUrl);
        console.log('🔗 Fallback API URLs:', this.apiUrls.slice(1));
        
        this.currentYear = new Date().getFullYear();
        this.currentMonth = new Date().getMonth() + 1;
        this.flightsData = {
            stats: {},
            calendar: {},
            upcoming: [],
            completed: [],
            all: []
        };
        this.currentLanguage = this.getLanguageCookie() || 'vn';
        
        console.log('🔧 MyFlightsManager constructor called');
        console.log('📍 API Base URL:', this.apiBaseUrl);
        console.log('🌐 Language:', this.currentLanguage);
        
        this.init();
    }

    init() {
        console.log('🔄 MyFlightsManager init() called');
        this.setupEventListeners();
        this.loadInitialData();
        this.setupLanguageSupport();
        console.log('✅ MyFlightsManager initialized');
    }

    setupEventListeners() {
        console.log('🔧 Setting up event listeners...');
        
        // Bootstrap tab change event
        document.addEventListener('shown.bs.tab', (event) => {
            const tabId = event.target.getAttribute('data-bs-target');
            console.log('📑 Tab changed to:', tabId);
            this.handleTabChange(tabId);
        });
        
        // Also listen for click events on tabs as backup
        const tabButtons = document.querySelectorAll('[data-bs-toggle="tab"]');
        tabButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const tabId = e.target.getAttribute('data-bs-target');
                console.log('🖱️ Tab clicked:', tabId);
                // Add small delay to ensure tab is shown
                setTimeout(() => {
                    this.handleTabChange(tabId);
                }, 100);
            });
        });

        const searchInput = document.getElementById('flightSearch');
        if (searchInput) {
            searchInput.addEventListener('input', this.debounce(() => {
                this.handleSearch();
            }, 500));

            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    this.handleSearch();
                }
            });
        }

        this.setupCalendarNavigation();
    }

    setupCalendarNavigation() {
        const prevBtn = document.getElementById('prevMonth');
        const nextBtn = document.getElementById('nextMonth');

        if (prevBtn) {
            prevBtn.addEventListener('click', () => this.navigateMonth(-1));
        }

        if (nextBtn) {
            nextBtn.addEventListener('click', () => this.navigateMonth(1));
        }
    }

    setupLanguageSupport() {
        const langSelect = document.getElementById('languageSelect');
        if (langSelect) {
            langSelect.value = this.currentLanguage;
            langSelect.addEventListener('change', (e) => {
                this.changeLanguage(e.target.value);
            });
        }

        this.updateLanguageElements();
    }

    updateLanguageElements() {
        if (typeof window.updateLanguage === 'function') {
            window.updateLanguage(this.currentLanguage);
        }
    }

    async loadInitialData() {
        console.log('=== MyFlights: Starting to load initial data ===');
        this.showLoading(true);
        
        try {
            // First, check authentication status - this will throw if auth fails
            const authStatus = await this.checkAuthenticationStatus();
            
            // If we get here, user is properly authenticated
            console.log('✅ Authentication confirmed, proceeding with data loading...');
            
            // Try to sync authentication from server to client domain (optional)
            await this.syncAuthenticationState();
            
            // Test the debug endpoint to see what cookies are available
            await this.testDebugEndpoint();
            
            // Load flight statistics and calendar data
            await this.loadFlightStats();
            await this.loadCalendarData(this.currentYear, this.currentMonth);
            
        } catch (error) {
            if (error.message.includes('redirecting to login')) {
                console.log('⚠️ Authentication required, stopping data load');
                return; // Stop execution, redirect is happening
            }
            
            console.error('Error loading initial data:', error);
            this.showError('Không thể tải dữ liệu. Vui lòng thử lại sau.');
            this.showLoading(false);
        }
    }

    async checkAuthenticationStatus() {
        try {
            console.log('🔍 Checking authentication status...');
            const response = await fetch('/MyFlights/api/auth-status', {
                method: 'GET',
                credentials: 'include'
            });
            
            if (response.ok) {
                const result = await response.json();
                console.log('🔍 Auth status result:', result);
                
                if (result.authenticated && result.hasToken) {
                    console.log('✅ User is authenticated with token on client domain');
                    return result;
                } else {
                    // User needs to login regardless of whether they're authenticated on server
                    console.log('⚠️ User needs to login on client domain');
                    this.showAuthenticationMessage();
                    
                    // Immediate redirect - don't wait
                    setTimeout(() => {
                        console.log('🔄 Redirecting to login page...');
                        window.location.href = '/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                    }, 1000);
                    
                    // Prevent further execution
                    throw new Error('Authentication required - redirecting to login');
                }
            } else {
                console.log('🔍 Auth status check failed:', response.status);
                this.showAuthenticationMessage();
                
                setTimeout(() => {
                    window.location.href = '/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                }, 1000);
                
                throw new Error('Authentication check failed - redirecting to login');
            }
        } catch (error) {
            console.log('🔍 Auth status check error:', error);
            if (!error.message.includes('redirecting to login')) {
                this.showAuthenticationMessage();
                setTimeout(() => {
                    window.location.href = '/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                }, 1000);
            }
            throw error;
        }
    }

    showAuthenticationMessage() {
        // Show a user-friendly message
        const messageHtml = `
            <div class="alert alert-info alert-dismissible fade show text-center" role="alert" style="margin: 2rem;">
                <div class="mb-3">
                    <i class="fas fa-plane fa-3x text-primary mb-3"></i>
                    <h4><strong>Chào mừng đến với Quản lý Chuyến bay</strong></h4>
                </div>
                <p class="mb-3">Để xem thông tin chuyến bay của bạn, vui lòng đăng nhập vào hệ thống.</p>
                <div class="mb-3">
                    <a href="/Login?returnUrl=${encodeURIComponent(window.location.pathname)}" class="btn btn-primary btn-lg">
                        <i class="fas fa-sign-in-alt me-2"></i>Đăng nhập để tiếp tục
                    </a>
                </div>
                <hr>
                <small class="text-muted">
                    <i class="fas fa-info-circle me-1"></i>
                    Bạn sẽ được chuyển hướng tự động trong giây lát...
                </small>
            </div>
        `;
        
        // Replace the entire page content with the auth message
        const container = document.querySelector('.container-fluid') || document.querySelector('.container') || document.body;
        
        // Clear existing content and show only auth message
        if (container) {
            container.innerHTML = messageHtml;
        }
        
        // Ensure loading is hidden
        this.showLoading(false);
    }

    async syncAuthenticationState() {
        try {
            console.log('🔄 Syncing authentication state from server to client...');
            const response = await fetch('/MyFlights/api/sync-auth', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const result = await response.json();
                console.log('✅ Auth sync result:', result);
                if (result.success) {
                    console.log('✅ Authentication state synchronized successfully');
                } else {
                    console.log('⚠️ Auth sync completed but no token found');
                }
            } else {
                console.log('⚠️ Auth sync failed, continuing anyway:', response.status);
            }
        } catch (error) {
            console.log('⚠️ Auth sync error, continuing anyway:', error);
        }
    }

    async testDebugEndpoint() {
        try {
            console.log('🔍 Testing debug endpoint to check cookies...');
            const response = await fetch('/MyFlights/api/debug', {
                method: 'GET',
                credentials: 'include'
            });
            
            if (response.ok) {
                const result = await response.json();
                console.log('🔍 Debug endpoint result:', result);
                console.log('🔍 Cookies seen by proxy:', result.cookies);
                console.log('🔍 Headers seen by proxy:', result.headers);
                
                // Check if X-Access-Token exists
                if (result.cookies && result.cookies['X-Access-Token']) {
                    console.log('✅ X-Access-Token cookie found in proxy!');
                } else {
                    console.log('❌ X-Access-Token cookie NOT found in proxy!');
                    console.log('Available cookies:', Object.keys(result.cookies || {}));
                }
            } else {
                console.log('🔍 Debug endpoint failed:', response.status);
            }
        } catch (error) {
            console.log('🔍 Debug endpoint error:', error);
        }
    }

    async loadFlightStats() {
        try {
            console.log('🚀 Loading flight stats...');
            
            // Try each API URL until one works
            let lastError = null;
            let success = false;
            
            for (let i = 0; i < this.apiUrls.length && !success; i++) {
                const apiUrl = this.apiUrls[i];
                const isProxy = apiUrl.includes('MyFlights');
                console.log(`🌐 Attempting API URL ${i + 1}/${this.apiUrls.length}: ${apiUrl} (${isProxy ? 'proxy' : 'direct'})`);
                
                try {
                    const token = await this.getAuthToken();
                    const isProxyAuth = token === 'AUTHENTICATED_VIA_PROXY';
                    console.log(`🔑 Token available: ${token ? 'YES' : 'NO'} (Proxy auth: ${isProxyAuth})`);
                    
                    const apiEndpoint = isProxy ? `${apiUrl}/stats` : `${apiUrl}/MyFlight/stats`;
                    
                    const headers = {
                        'Content-Type': 'application/json'
                    };
                    
                    // Add Authorization header only if we have a real JWT token (not proxy auth)
                    if (token && !isProxyAuth) {
                        headers['Authorization'] = `Bearer ${token}`;
                    }
                    
                    const response = await fetch(apiEndpoint, {
                        method: 'GET',
                        headers: headers,
                        credentials: 'include'  // Always include cookies for proxy
                    });

                    console.log(`📊 Stats API response status (${apiUrl}):`, response.status);
                    
                    if (response.ok) {
                        const result = await response.json();
                        console.log('📈 Stats API result:', result);
                        
                        if (result.success) {
                            console.log(`✅ API call succeeded with ${apiUrl}!`);
                            this.apiBaseUrl = apiUrl; // Switch permanently to working URL
                            this.flightsData.stats = result.data;
                            this.updateStatsDisplay();
                            console.log('✅ Flight stats loaded successfully');
                            success = true;
                            break;
                        } else {
                            console.error('API returned success=false:', result);
                            lastError = new Error('API trả về lỗi: ' + (result.message || 'Unknown error'));
                        }
                    } else if (response.status === 401) {
                        console.error(`🔐 401 Unauthorized from ${apiUrl}`);
                        const responseText = await response.text();
                        lastError = {
                            is401: true,
                            status: response.status,
                            responseText,
                            apiUrl,
                            isProxy
                        };
                    } else {
                        const responseText = await response.text();
                        lastError = new Error(`HTTP error! status: ${response.status}, body: ${responseText}`);
                    }
                } catch (fetchError) {
                    console.error(`❌ Error with API URL ${apiUrl}:`, fetchError);
                    lastError = fetchError;
                }
            }
            
            // If all URLs failed, show error
            if (!success) {
                if (lastError && lastError.is401) {
                    this.showAuthenticationError(lastError);
                } else {
                    throw lastError || new Error('All API URLs failed');
                }
            }
        } catch (error) {
            console.error('Error loading flight stats:', error);
            this.showError('Không thể tải thống kê chuyến bay. Lỗi: ' + error.message);
        }
    }

    showAuthenticationError(error) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger text-center';
        errorDiv.innerHTML = `
            <h5><i class="fas fa-exclamation-triangle"></i> Cần đăng nhập</h5>
            <p><strong>Error:</strong> ${error.responseText || 'Không có quyền truy cập'}</p>
            <p>Vui lòng đăng nhập để xem thông tin chuyến bay của bạn.</p>
            <a href="/Login" class="btn btn-primary">
                <i class="fas fa-sign-in-alt"></i> Đăng nhập ngay
            </a>
        `;
        
        const container = document.querySelector('.container-fluid') || document.body;
        const existing = container.querySelector('.alert-danger, .alert-warning');
        if (existing) existing.remove();
        container.insertBefore(errorDiv, container.firstChild);
        
        this.showLoading(false);
    }

    updateStatsDisplay() {
        const stats = this.flightsData.stats;
        
        const upcomingElement = document.getElementById('upcomingCount');
        if (upcomingElement) upcomingElement.textContent = stats.upcomingFlights || 0;
        
        const completedElement = document.getElementById('completedCount');
        if (completedElement) completedElement.textContent = stats.completedFlights || 0;
        
        const totalElement = document.getElementById('totalCount');
        if (totalElement) totalElement.textContent = stats.totalFlights || 0;
        
        const spentElement = document.getElementById('totalSpent');
        if (spentElement) spentElement.textContent = this.formatCurrency(stats.totalSpent || 0);
    }

    async loadCalendarData(year, month) {
        try {
            console.log(`🗓️ Loading calendar data for ${year}/${month}`);
            
            let success = false;
            
            for (let i = 0; i < this.apiUrls.length && !success; i++) {
                const apiUrl = this.apiUrls[i];
                const isProxy = apiUrl.includes('MyFlights');
                console.log(`🌐 Attempting calendar API URL ${i + 1}/${this.apiUrls.length}: ${apiUrl} (${isProxy ? 'proxy' : 'direct'})`);
                
                try {
                    const token = await this.getAuthToken();
                    const isProxyAuth = token === 'AUTHENTICATED_VIA_PROXY';
                    console.log(`🔑 Token available for calendar: ${token ? 'YES' : 'NO'} (Proxy auth: ${isProxyAuth})`);
                    
                    const apiEndpoint = isProxy ? `${apiUrl}/calendar?year=${year}&month=${month}` : `${apiUrl}/MyFlight/calendar?year=${year}&month=${month}`;
                    
                    const headers = {
                        'Content-Type': 'application/json'
                    };
                    
                    // Add Authorization header only if we have a real JWT token (not proxy auth)
                    if (token && !isProxyAuth) {
                        headers['Authorization'] = `Bearer ${token}`;
                    }
                    
                    const response = await fetch(apiEndpoint, {
                        method: 'GET',
                        headers: headers,
                        credentials: 'include'
                    });

                    console.log(`📅 Calendar API response status (${apiUrl}):`, response.status);
                    
                    if (response.ok) {
                        const result = await response.json();
                        if (result.success) {
                            console.log(`✅ Calendar API call succeeded with ${apiUrl}!`);
                            this.apiBaseUrl = apiUrl;
                            this.renderCalendar(result.data);
                            success = true;
                            break;
                        }
                    } else if (response.status === 401) {
                        console.error(`🔐 401 Unauthorized from calendar API ${apiUrl}`);
                        // Continue to next URL
                    } else {
                        console.error(`❌ Calendar API error from ${apiUrl}: ${response.status}`);
                    }
                } catch (error) {
                    console.error(`❌ Error with calendar API URL ${apiUrl}:`, error);
                }
            }
            
            if (!success) {
                console.error('❌ All calendar API URLs failed');
            }
            
            this.updateCalendarTitle(year, month);
        } catch (error) {
            console.error('Error loading calendar data:', error);
            this.showError('Không thể tải dữ liệu lịch.');
        }
    }

    renderCalendar(calendarData) {
        const calendarGrid = document.getElementById('calendarGrid');
        if (!calendarGrid) return;

        this.flightsData.calendar = calendarData;
        // Save full flight list for other tabs (if provided)
        if (Array.isArray(calendarData.allFlights)) {
            this.flightsData.all = calendarData.allFlights;
            const now = new Date();
            this.flightsData.upcoming = calendarData.allFlights.filter(f => new Date(f.departureTime) > now);
            this.flightsData.completed = calendarData.allFlights.filter(f => new Date(f.departureTime) <= now);
        }
        calendarGrid.innerHTML = '';

        const daysOfWeek = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
        daysOfWeek.forEach(day => {
            const dayHeader = document.createElement('div');
            dayHeader.className = 'calendar-day-header';
            dayHeader.textContent = day;
            calendarGrid.appendChild(dayHeader);
        });

        const firstDay = new Date(calendarData.year, calendarData.month - 1, 1);
        const lastDay = new Date(calendarData.year, calendarData.month, 0);
        const firstDayOfWeek = firstDay.getDay();
        const daysInMonth = lastDay.getDate();

        for (let i = 0; i < firstDayOfWeek; i++) {
            const emptyDay = document.createElement('div');
            emptyDay.className = 'calendar-day empty';
            calendarGrid.appendChild(emptyDay);
        }

        for (let day = 1; day <= daysInMonth; day++) {
            const dayElement = document.createElement('div');
            const dateString = `${calendarData.year}-${String(calendarData.month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
            
            dayElement.className = 'calendar-day';
            dayElement.setAttribute('data-date', dateString);
            if (this.isToday(dateString)) {
                dayElement.classList.add('today');
            }

            const dayData = calendarData.calendarDays.find(d => 
                new Date(d.date).toDateString() === new Date(dateString).toDateString()
            );

            let dayContent = `<div class="day-number">${day}</div>`;
            
            if (dayData && dayData.hasFlights) {
                dayElement.classList.add('has-flights');
                dayContent += `<div class="flight-count">${dayData.flightCount} chuyến</div>`;
            }

            dayElement.innerHTML = dayContent;
            calendarGrid.appendChild(dayElement);
        }
        
        this.showLoading(false);
    }

    isToday(dateString) {
        const today = new Date().toDateString();
        const compareDate = new Date(dateString).toDateString();
        return today === compareDate;
    }

    updateCalendarTitle(year, month) {
        const titleElement = document.getElementById('calendarTitle');
        if (titleElement) {
            const monthNames = [
                'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
            ];
            titleElement.textContent = `${monthNames[month - 1]} ${year}`;
        }

        this.currentYear = year;
        this.currentMonth = month;
    }

    navigateMonth(direction) {
        let newMonth = this.currentMonth + direction;
        let newYear = this.currentYear;

        if (newMonth < 1) {
            newMonth = 12;
            newYear--;
        } else if (newMonth > 12) {
            newMonth = 1;
            newYear++;
        }

        this.loadCalendarData(newYear, newMonth);
    }

    /* ==================== TAB HANDLING & FLIGHT LISTS ==================== */

    async handleTabChange(tabId) {
        console.log('🔄 Handling tab change for:', tabId);
        try {
            switch (tabId) {
                case '#upcoming':
                    console.log('📈 Loading upcoming flights...');
                    await this.loadUpcomingFlights();
                    break;
                case '#completed':
                    console.log('✅ Loading completed flights...');
                    await this.loadCompletedFlights();
                    break;
                case '#all':
                    console.log('📋 Loading all flights...');
                    await this.loadAllFlights();
                    break;
                default:
                    console.log('ℹ️ Tab not handled:', tabId);
            }
        } catch (error) {
            console.error('❌ Error in handleTabChange:', error);
        }
    }

    async loadUpcomingFlights() {
        if (this.flightsData.upcomingLoaded) return;
        const containerId = 'upcomingFlights';
        this.showListLoading(containerId, true);
        try {
            const response = await fetch('/MyFlights/api/upcoming', { credentials: 'include' });
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.flightsData.upcoming = result.data;
                    this.renderFlights(containerId, result.data);
                    this.flightsData.upcomingLoaded = true;
                }
            } else {
                const txt = await response.text();
                console.error('Upcoming flights API error:', txt);
            }
        } catch (err) {
            console.error('Upcoming flights fetch error:', err);
        }
        this.showListLoading(containerId, false);
    }

    async loadCompletedFlights() {
        if (this.flightsData.completedLoaded) return;
        const containerId = 'completedFlights';
        this.showListLoading(containerId, true);
        try {
            const response = await fetch('/MyFlights/api/completed', { credentials: 'include' });
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.flightsData.completed = result.data;
                    this.renderFlights(containerId, result.data);
                    this.flightsData.completedLoaded = true;
                }
            } else {
                const txt = await response.text();
                console.error('Completed flights API error:', txt);
            }
        } catch (err) {
            console.error('Completed flights fetch error:', err);
        }
        this.showListLoading(containerId, false);
    }

    async loadAllFlights() {
        if (this.flightsData.allLoaded) return;
        const containerId = 'allFlights';
        this.showListLoading(containerId, true);
        try {
            const response = await fetch('/MyFlights/api/all', { credentials: 'include' });
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.flightsData.all = result.data;
                    this.renderFlights(containerId, result.data);
                    this.flightsData.allLoaded = true;
                }
            } else {
                const txt = await response.text();
                console.error('All flights API error:', txt);
            }
        } catch (err) {
            console.error('All flights fetch error:', err);
        }
        this.showListLoading(containerId, false);
    }

    showListLoading(containerId, show) {
        const container = document.getElementById(containerId);
        if (!container) return;
        if (show) {
            container.innerHTML = `<div class="loading-spinner"><i class="fas fa-plane fa-spin"></i></div>`;
        }
    }

    renderFlights(containerId, flights) {
        const container = document.getElementById(containerId);
        if (!container) return;

        // Remove loading spinner
        container.innerHTML = '';

        if (!flights || flights.length === 0) {
            container.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-plane-slash fa-3x mb-4"></i>
                    <h5 style="color: var(--text-secondary); font-weight: 600;">Không có chuyến bay</h5>
                    <p style="color: var(--text-secondary); margin: 0;">Chưa có chuyến bay nào trong danh mục này</p>
                </div>`;
            return;
        }

        const flightCardsHtml = flights.map(flight => {
            const depTime = new Date(flight.departureTime);
            const arrTime = new Date(flight.arrivalTime);
            const depStr = depTime.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
            const arrStr = arrTime.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
            const depDate = depTime.toLocaleDateString('vi-VN', { 
                weekday: 'short', 
                day: '2-digit', 
                month: '2-digit',
                year: 'numeric'
            });
            
            // Premium status styling
            const statusClass = this.getStatusClass(flight.statusName);
            const statusDisplay = this.getStatusDisplay(flight.statusName);
            
            // Format price
            const priceFormatted = new Intl.NumberFormat('vi-VN', {
                style: 'currency',
                currency: 'VND',
                minimumFractionDigits: 0
            }).format(flight.totalPrice);
            
            // Class styling
            const classIcon = this.getClassIcon(flight.className);
            const duration = this.calculateDuration(depTime, arrTime);
            
            return `
                <div class="flight-card" onclick="window.myFlightsManager.showFlightDetail(${flight.ticketId})" style="cursor: pointer;">
                    <div class="flight-header d-flex justify-content-between align-items-center">
                        <div class="flight-code">${this.formatFlightCode(flight.flightCode)}</div>
                        <div class="flight-status ${this.getStatusClass(flight.statusName)}">${this.getStatusDisplay(flight.statusName)}</div>
                    </div>
                    
                    <div class="flight-route">
                        <div class="airport">
                            <div class="airport-code">${flight.departureAirportCode}</div>
                            <div class="airport-name">${this.truncateText(flight.departureAirportName, 20)}</div>
                        </div>
                        <div class="flight-arrow">
                            <i class="fas fa-plane"></i>
                        </div>
                        <div class="airport">
                            <div class="airport-code">${flight.arrivalAirportCode}</div>
                            <div class="airport-name">${this.truncateText(flight.arrivalAirportName, 20)}</div>
                        </div>
                    </div>
                    
                    <div class="flight-time">
                        <div>
                            <small class="text-muted">Khởi hành:</small><br>
                            <strong>${depDate} - ${depStr}</strong>
                        </div>
                        <div class="text-center">
                            <small class="text-muted">Thời gian bay:</small><br>
                            <span>${duration}</span>
                        </div>
                        <div class="text-end">
                            <div class="d-flex align-items-center justify-content-end mb-1">
                                ${classIcon}
                                <span class="ms-1 fw-semibold">${flight.className}</span>
                            </div>
                            <div class="price">${priceFormatted}</div>
                        </div>
                    </div>
                </div>
`;
        }).join('');

        container.innerHTML = flightCardsHtml;
    }

    truncateText(text, maxLength) {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    }

    getStatusClass(statusName) {
        const status = statusName.toLowerCase();
        if (status.includes('active') || status.includes('confirmed')) return 'active';
        if (status.includes('cancelled') || status.includes('canceled')) return 'cancelled';
        if (status.includes('completed') || status.includes('finished')) return 'confirmed';
        return 'active';
    }
    
    getStatusDisplay(statusName) {
        const status = statusName.toLowerCase();
        if (status.includes('active')) return 'Đã xác nhận';
        if (status.includes('cancelled')) return 'Đã hủy';
        if (status.includes('completed')) return 'Hoàn thành';
        return statusName;
    }
    
    getClassIcon(className) {
        const classLower = className.toLowerCase();
        if (classLower.includes('first') || classLower.includes('elite')) return '<i class="fas fa-crown"></i>';
        if (classLower.includes('business') || classLower.includes('premium')) return '<i class="fas fa-star"></i>';
        return '<i class="fas fa-chair"></i>';
    }
    
    formatFlightCode(code) {
        // Format long flight codes nicely
        if (code && code.length > 8) {
            return code.substring(0, 4) + ' ' + code.substring(4, 8) + ' ' + code.substring(8);
        }
        return code;
    }
    
    calculateDuration(depTime, arrTime) {
        const diff = arrTime - depTime;
        const hours = Math.floor(diff / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        return `${hours}h ${minutes}m`;
    }

    async showFlightDetail(ticketId) {
        try {
            console.log('🔍 Loading flight detail for ticket:', ticketId);
            
            // Find flight in cached data first
            let flight = null;
            for (const flightList of [this.flightsData.upcoming, this.flightsData.completed, this.flightsData.all]) {
                flight = flightList.find(f => f.ticketId === ticketId);
                if (flight) break;
            }
            
            if (!flight) {
                // If not found in cache, fetch from API
                const response = await fetch(`/MyFlights/api/detail/${ticketId}`, { credentials: 'include' });
                if (response.ok) {
                    const result = await response.json();
                    if (result.success) {
                        flight = result.data;
                    }
                } else {
                    console.error('Failed to fetch flight detail');
                    return;
                }
            }
            
            if (!flight) {
                console.error('Flight not found');
                return;
            }
            
            // Create modal content
            const modalContent = this.generateFlightDetailModal(flight);
            
            // Update existing modal or create new one
            const existingModal = document.getElementById('flightDetailModal');
            if (existingModal) {
                const modalBody = existingModal.querySelector('#flightDetailContent');
                modalBody.innerHTML = modalContent;
                
                const bsModal = bootstrap.Modal.getOrCreateInstance(existingModal);
                bsModal.show();
            } else {
                console.error('Flight detail modal not found in DOM');
            }
            
        } catch (error) {
            console.error('❌ Error showing flight detail:', error);
        }
    }
    
    generateFlightDetailModal(flight) {
        const depDate = new Date(flight.departureTime);
        const arrDate = new Date(flight.arrivalTime);
        const depTime = depDate.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        const arrTime = arrDate.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        const duration = this.calculateDuration(depDate, arrDate);
        const classIcon = this.getClassIcon(flight.className);
        const totalPrice = this.formatCurrency(flight.totalPrice);
        const bookingDate = new Date(flight.bookingDate).toLocaleDateString('vi-VN');
        
        return `
            <div class="flight-detail-content">
                <!-- Flight Header -->
                <div class="d-flex justify-content-between align-items-center mb-4 p-3 rounded" style="background: linear-gradient(135deg, var(--airline-navy), var(--airline-blue)); color: white;">
                    <div>
                        <h4 class="mb-1">${this.formatFlightCode(flight.flightCode)}</h4>
                        <p class="mb-0 opacity-75">${flight.ticketNumber}</p>
                    </div>
                    <div class="text-end">
                        <span class="badge bg-light text-dark px-3 py-2">${this.getStatusDisplay(flight.statusName)}</span>
                    </div>
                </div>
                
                <!-- Flight Route -->
                <div class="row mb-4">
                    <div class="col-5">
                        <div class="text-center p-3 border rounded">
                            <h2 class="mb-1" style="color: var(--airline-navy); font-weight: 800;">${flight.departureAirportCode}</h2>
                            <p class="mb-2 text-muted small">${flight.departureAirportName}</p>
                            <div class="fw-bold" style="color: var(--airline-blue);">
                                ${depDate.toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: '2-digit' })}
                            </div>
                            <div class="h5 mb-0">${depTime}</div>
                        </div>
                    </div>
                    <div class="col-2 d-flex align-items-center justify-content-center">
                        <div class="text-center">
                            <i class="fas fa-plane fa-2x mb-2" style="color: var(--airline-gold);"></i>
                            <div class="small text-muted">${duration}</div>
                        </div>
                    </div>
                    <div class="col-5">
                        <div class="text-center p-3 border rounded">
                            <h2 class="mb-1" style="color: var(--airline-navy); font-weight: 800;">${flight.arrivalAirportCode}</h2>
                            <p class="mb-2 text-muted small">${flight.arrivalAirportName}</p>
                            <div class="fw-bold" style="color: var(--airline-blue);">
                                ${arrDate.toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: '2-digit' })}
                            </div>
                            <div class="h5 mb-0">${arrTime}</div>
                        </div>
                    </div>
                </div>
                
                <!-- Flight Details -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="card border-0 shadow-sm mb-3">
                            <div class="card-header bg-light border-0">
                                <h6 class="mb-0"><i class="fas fa-user me-2"></i>Thông tin hành khách</h6>
                            </div>
                            <div class="card-body">
                                <div class="mb-2">
                                    <strong>Họ tên:</strong> ${flight.fullName}
                                </div>
                                <div class="mb-2">
                                    <strong>Giới tính:</strong> ${flight.gender === 'male' ? 'Nam' : 'Nữ'}
                                </div>
                                <div class="mb-2">
                                    <strong>Ngày sinh:</strong> ${new Date(flight.dateOfBirth).toLocaleDateString('vi-VN')}
                                </div>
                                ${flight.seatNumber ? `<div class="mb-2">
                                    <strong>Số ghế:</strong> <span class="badge bg-primary">${flight.seatNumber}</span>
                                </div>` : ''}
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="card border-0 shadow-sm mb-3">
                            <div class="card-header bg-light border-0">
                                <h6 class="mb-0"><i class="fas fa-plane me-2"></i>Chi tiết chuyến bay</h6>
                            </div>
                            <div class="card-body">
                                <div class="mb-2">
                                    <strong>Hạng ghế:</strong> ${classIcon} ${flight.className}
                                </div>
                                <div class="mb-2">
                                    <strong>Ngày đặt:</strong> ${bookingDate}
                                </div>
                                <div class="mb-2">
                                    <strong>Tổng tiền:</strong> <span class="fw-bold text-primary">${totalPrice}</span>
                                </div>
                                <div class="mb-2">
                                    <strong>Trạng thái:</strong> <span class="badge bg-success">${this.getStatusDisplay(flight.statusName)}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                ${flight.contactFullName ? `
                <!-- Contact Information -->
                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-light border-0">
                        <h6 class="mb-0"><i class="fas fa-address-book me-2"></i>Thông tin liên hệ</h6>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <strong>Người liên hệ:</strong> ${flight.contactFullName}
                            </div>
                            ${flight.contactPhone ? `<div class="col-md-4">
                                <strong>Điện thoại:</strong> ${flight.contactPhone}
                            </div>` : ''}
                            ${flight.contactEmail ? `<div class="col-md-4">
                                <strong>Email:</strong> ${flight.contactEmail}
                            </div>` : ''}
                        </div>
                    </div>
                </div>
                ` : ''}
                
                ${flight.ticketServices && flight.ticketServices.length > 0 ? `
                <!-- Additional Services -->
                <div class="card border-0 shadow-sm mt-3">
                    <div class="card-header bg-light border-0">
                        <h6 class="mb-0"><i class="fas fa-concierge-bell me-2"></i>Dịch vụ bổ sung</h6>
                    </div>
                    <div class="card-body">
                        ${flight.ticketServices.map(service => `
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span>${service.serviceName}</span>
                                <span class="fw-bold">${this.formatCurrency(service.price)}</span>
                            </div>
                        `).join('')}
                    </div>
                </div>
                ` : ''}
            </div>
        `;
    }

    handleSearch() {
        const searchTerm = document.getElementById('flightSearch')?.value;
        if (!searchTerm) return;
        this.searchFlights(searchTerm);
    }

    async searchFlights(searchTerm) {
        try {
            const token = await this.getAuthToken();
            const response = await fetch(`${this.apiBaseUrl}/MyFlight/search?term=${encodeURIComponent(searchTerm)}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to search flights');
            }

            const result = await response.json();
            if (result.success) {
                console.log('Search results:', result.data);
            }
        } catch (error) {
            console.error('Error searching flights:', error);
        }
    }

    showLoading(show) {
        const loadingElement = document.getElementById('loadingSpinner');
        if (loadingElement) {
            loadingElement.style.display = show ? 'flex' : 'none';
        }
        console.log('Loading:', show);
    }

    showError(message) {
        console.error('MyFlights Error:', message);
        alert(message);
    }

    async getAuthToken() {
        console.log('=== MyFlights GET AUTH TOKEN DEBUG ===');
        
        // First check if user is authenticated by calling the same endpoint as Header.js
        try {
            const host = window.location.hostname;
            const decodedTokenResponse = await fetch(`http://${host}:5077/api/Token/get`, { 
                method: 'GET', 
                credentials: 'include' 
            });
            
            if (decodedTokenResponse.ok) {
                const decodedToken = await decodedTokenResponse.json();
                console.log('✅ User is authenticated! Server returned token:', decodedToken);
                
                // Since user is authenticated, we can rely on proxy approach
                // The proxy controller will forward cookies automatically
                console.log('✅ Using proxy-based authentication (no raw token needed)');
                console.log('=== END MyFlights GET AUTH TOKEN DEBUG ===');
                return 'AUTHENTICATED_VIA_PROXY'; // Special value to indicate proxy auth
            } else {
                console.log('❌ User is NOT authenticated, server returned:', decodedTokenResponse.status);
                console.log('=== END MyFlights GET AUTH TOKEN DEBUG ===');
                return null; // User not authenticated
            }
        } catch (error) {
            console.log('❌ Error checking authentication:', error);
            console.log('=== END MyFlights GET AUTH TOKEN DEBUG ===');
            return null;
        }
    }

    getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return null;
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    getLanguageCookie() {
        return this.getCookie('selectedLanguage');
    }

    changeLanguage(language) {
        this.currentLanguage = language;
        document.cookie = `selectedLanguage=${language}; path=/`;
        this.updateLanguageElements();
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 DOM loaded, initializing MyFlights...');
    
    // Check if we have the required elements
    const requiredElements = ['loadingSpinner', 'upcomingCount', 'completedCount', 'totalCount', 'totalSpent', 'calendarGrid', 'calendarTitle'];
    const missingElements = requiredElements.filter(id => !document.getElementById(id));
    
    if (missingElements.length > 0) {
        console.warn('⚠️ Some required elements are missing:', missingElements);
    }
    
    // Initialize MyFlights manager
    window.myFlightsManager = new MyFlightsManager();
    console.log('✅ MyFlights manager initialized and attached to window');
});

console.log('📄 MyFlights.js script loaded successfully');
