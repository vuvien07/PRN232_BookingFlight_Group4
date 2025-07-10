// Manager Services JavaScript
let currentPage = 1;
const pageSize = 10;
let currentSearchTerm = '';
let currentStatusFilter = '';

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    initializeServiceManagement();
});

function initializeServiceManagement() {
    console.log('Initializing service management...');
    
    // Setup search functionality
    setupSearchAndFilters();
    
    // Load services on page load
    loadServices();
    
    // Setup refresh button
    const refreshBtn = document.querySelector('[onclick="loadServices()"]');
    if (refreshBtn) {
        refreshBtn.addEventListener('click', function(e) {
            e.preventDefault();
            loadServices();
        });
    }
}

function setupSearchAndFilters() {
    // Search input
    const searchInput = document.getElementById('searchServices');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            currentSearchTerm = this.value;
            currentPage = 1;
            loadServices();
        });
    }
    
    // Status filter
    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) {
        statusFilter.addEventListener('change', function() {
            currentStatusFilter = this.value;
            currentPage = 1;
            loadServices();
        });
        
        // Populate status filter options
        populateStatusFilter();
    }
}

function populateStatusFilter() {
    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) {
        statusFilter.innerHTML = `
            <option value="">Tất cả trạng thái</option>
            <option value="1">Hoạt động</option>
            <option value="2">Không hoạt động</option>
        `;
    }
}

async function loadServices() {
    console.log('Loading services...');
    
    try {
        // Show loading state
        showLoadingState();
        
        // Prepare request data
        const requestData = {
            page: currentPage,
            pageSize: pageSize,
            searchTerm: currentSearchTerm,
            statusId: currentStatusFilter ? parseInt(currentStatusFilter) : null,
            managerId: null // Allow all managers for now
        };
        
        console.log('Request data:', requestData);
        
        // Make API call
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.SERVICES.LIST), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include',
            body: JSON.stringify(requestData)
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                showErrorState('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            throw new Error(`Server responded with status ${response.status}`);
        }
        
        const result = await response.json();
        console.log('API Response:', result);
        
        if (result.success) {
            displayServices(result.data || []);
            updatePagination(result.pagination?.totalCount || 0, result.pagination?.totalPages || 1);
        } else {
            throw new Error(result.message || 'Failed to load services');
        }
        
    } catch (error) {
        console.error('Error loading services:', error);
        
        if (error.message.includes('Failed to fetch') || error.message.includes('ERR_')) {
            showErrorState('Không thể kết nối tới server. Vui lòng kiểm tra xem API server có đang chạy không.');
        } else {
            showErrorState(error.message);
        }
    }
}

function useMockData() {
    // Mock data for testing
    const mockServices = [
        {
            id: 1,
            name: 'Flight Booking Service',
            description: 'Handles all flight booking operations and reservations',
            status: 'active',
            createdDate: '2024-01-15T10:00:00Z',
            managerName: 'John Manager'
        },
        {
            id: 2,
            name: 'Payment Processing',
            description: 'Secure payment processing for all transactions',
            status: 'active',
            createdDate: '2024-01-20T14:30:00Z',
            managerName: 'Sarah Payment'
        },
        {
            id: 3,
            name: 'Customer Support',
            description: 'Customer service and support ticket management',
            status: 'maintenance',
            createdDate: '2024-02-01T09:15:00Z',
            managerName: 'Mike Support'
        },
        {
            id: 4,
            name: 'Email Notifications',
            description: 'Automated email notifications for bookings and updates',
            status: 'pending',
            createdDate: '2024-02-10T16:45:00Z',
            managerName: 'Lisa Email'
        },
        {
            id: 5,
            name: 'Database Backup',
            description: 'Automated database backup and recovery service',
            status: 'inactive',
            createdDate: '2024-01-05T08:00:00Z',
            managerName: 'Tom Database'
        }
    ];
    
    // Filter mock data based on search and status
    let filteredServices = mockServices;
    
    if (currentSearchTerm) {
        filteredServices = filteredServices.filter(service => 
            service.name.toLowerCase().includes(currentSearchTerm.toLowerCase()) ||
            service.description.toLowerCase().includes(currentSearchTerm.toLowerCase())
        );
    }
    
    if (currentStatusFilter) {
        filteredServices = filteredServices.filter(service => 
            service.status.toLowerCase() === currentStatusFilter.toLowerCase()
        );
    }
    
    // Pagination
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedServices = filteredServices.slice(startIndex, endIndex);
    
    const totalPages = Math.ceil(filteredServices.length / pageSize);
    
    displayServices(paginatedServices);
    updatePagination(filteredServices.length, totalPages);
}

function showLoadingState() {
    const loadingDiv = document.getElementById('servicesLoading');
    const containerDiv = document.getElementById('servicesContainer');
    const noServicesDiv = document.getElementById('noServicesMessage');
    const paginationDiv = document.getElementById('servicesPagination');
    
    if (loadingDiv) loadingDiv.style.display = 'block';
    if (containerDiv) containerDiv.style.display = 'none';
    if (noServicesDiv) noServicesDiv.style.display = 'none';
    if (paginationDiv) paginationDiv.style.display = 'none';
}

function displayServices(services) {
    const loadingDiv = document.getElementById('servicesLoading');
    const containerDiv = document.getElementById('servicesContainer');
    const noServicesDiv = document.getElementById('noServicesMessage');
    
    console.log('displayServices called with:', services);
    console.log('Container div:', containerDiv);
    
    // Hide loading
    if (loadingDiv) loadingDiv.style.display = 'none';
    
    if (!services || services.length === 0) {
        // Show no services message
        if (noServicesDiv) noServicesDiv.style.display = 'block';
        if (containerDiv) containerDiv.style.display = 'none';
        return;
    }
    
    // Show services container
    if (containerDiv) {
        containerDiv.style.display = 'block';
        const serviceCardsHtml = services.map(service => createServiceCard(service)).join('');
        console.log('Generated HTML:', serviceCardsHtml);
        containerDiv.innerHTML = serviceCardsHtml;
    }
    
    if (noServicesDiv) noServicesDiv.style.display = 'none';
}

function createServiceCard(service) {
    const statusClass = getStatusClass(service.status?.statusName || 'Unknown');
    const statusText = getStatusText(service.status?.statusName || 'Unknown');
    
    return `
        <div class="col-md-4 mb-4">
            <div class="card service-card h-100">
                <div class="card-body d-flex flex-column">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                        <h5 class="card-title">${escapeHtml(service.serviceName || 'Unnamed Service')}</h5>
                        <span class="badge ${statusClass}">${statusText}</span>
                    </div>
                    
                    <div class="service-info mb-3 flex-grow-1">
                        <p class="card-text text-muted mb-3">
                            ${escapeHtml(service.detail || 'No description available')}
                        </p>
                        <small class="text-muted">
                            <i class="fas fa-user me-1"></i>
                            Manager: ${escapeHtml(service.managerName || 'Not assigned')}
                        </small>
                        <br>
                        <small class="text-muted">
                            <i class="fas fa-plane me-1"></i>
                            Flights: ${service.flightCount || 0}
                        </small>
                        <br>
                        <small class="text-muted">
                            <i class="fas fa-box me-1"></i>
                            Items: ${service.itemCount || 0}
                        </small>
                    </div>
                    
                    <div class="d-flex justify-content-center align-items-center mt-auto">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-info" onclick="viewServiceDetails(${service.serviceId})" title="View Details">
                                <i class="fas fa-eye"></i>
                            </button>
                            <button class="btn btn-outline-primary" onclick="editService(${service.serviceId})" title="Edit Service">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-outline-danger" onclick="deleteService(${service.serviceId})" title="Delete Service">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

function getStatusClass(status) {
    switch (status?.toLowerCase()) {
        case 'active': return 'bg-success';
        case 'inactive': return 'bg-secondary';
        case 'pending': return 'bg-warning';
        case 'maintenance': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

function getStatusText(status) {
    return status || 'Unknown';
}

function updatePagination(totalCount, totalPages) {
    const paginationDiv = document.getElementById('servicesPagination');
    const paginationList = document.getElementById('paginationList');
    
    if (!paginationDiv || !paginationList) return;
    
    if (totalPages <= 1) {
        paginationDiv.style.display = 'none';
        return;
    }
    
    paginationDiv.style.display = 'block';
    
    let paginationHtml = '';
    
    // Previous button
    if (currentPage > 1) {
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${currentPage - 1})">Previous</a>
            </li>
        `;
    }
    
    // Page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage) {
            paginationHtml += `
                <li class="page-item active">
                    <span class="page-link">${i}</span>
                </li>
            `;
        } else {
            paginationHtml += `
                <li class="page-item">
                    <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                </li>
            `;
        }
    }
    
    // Next button
    if (currentPage < totalPages) {
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${currentPage + 1})">Next</a>
            </li>
        `;
    }
    
    paginationList.innerHTML = paginationHtml;
}

function changePage(page) {
    currentPage = page;
    loadServices();
}

function showErrorState(message) {
    const loadingDiv = document.getElementById('servicesLoading');
    const containerDiv = document.getElementById('servicesContainer');
    const noServicesDiv = document.getElementById('noServicesMessage');
    
    // Hide loading
    if (loadingDiv) loadingDiv.style.display = 'none';
    if (containerDiv) containerDiv.style.display = 'none';
    
    // Show error message
    if (noServicesDiv) {
        noServicesDiv.style.display = 'block';
        noServicesDiv.innerHTML = `
            <i class="fas fa-exclamation-triangle fa-3x text-danger mb-3"></i>
            <h5 class="text-danger">Error Loading Services</h5>
            <p class="text-muted">${escapeHtml(message)}</p>
            <button class="btn btn-primary" onclick="loadServices()">
                <i class="fas fa-retry me-2"></i>Try Again
            </button>
        `;
    }
}

// Service management functions
function addNewService() {
    // Navigate to Add Service page
    window.location.href = '/Manager/AddService';
}

function editService(serviceId) {
    // TODO: Implement edit service modal/form
    console.log('Edit service:', serviceId);
    alert(`Edit Service ${serviceId} functionality will be implemented`);
}

function viewService(serviceId) {
    // Navigate to service details page
    window.location.href = `/Manager/ServiceDetails?id=${serviceId}`;
}

function viewServiceDetails(serviceId) {
    // Navigate to service details page
    window.location.href = `/Manager/ServiceDetails?id=${serviceId}`;
}

function deleteService(serviceId) {
    if (confirm('Are you sure you want to delete this service?')) {
        // TODO: Implement delete service API call
        console.log('Delete service:', serviceId);
        alert(`Delete Service ${serviceId} functionality will be implemented`);
    }
}

// Support ticket functions (for supporter role)
function createNewTicket() {
    // TODO: Implement create ticket modal/form
    console.log('Create new ticket clicked');
    alert('Create Ticket functionality will be implemented');
}

// Utility functions
function getAuthToken() {
    // Try to get JWT token from cookies first, then localStorage, sessionStorage
    const cookieToken = getCookie('X-Access-Token');
    if (cookieToken) {
        return cookieToken;
    }
    
    return localStorage.getItem('authToken') || 
           sessionStorage.getItem('authToken') || 
           localStorage.getItem('token') || 
           sessionStorage.getItem('token') || '';
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    if (!dateString) return 'N/A';
    
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    } catch (error) {
        return 'Invalid Date';
    }
}

// Global functions for onclick handlers
window.loadServices = loadServices;
window.addNewService = addNewService;
window.editService = editService;
window.viewService = viewService;
window.viewServiceDetails = viewServiceDetails;
window.deleteService = deleteService;
window.createNewTicket = createNewTicket;
window.changePage = changePage;
