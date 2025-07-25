// Calendar Extensions for advanced functionality
class CalendarExtensions {
    constructor() {
        this.currentView = 'month'; // month, week, day
        this.exportFormat = 'pdf'; // pdf, excel, json
    }

    // Export calendar functionality
    exportCalendar() {
        const year = window.myFlightsManager?.currentYear || new Date().getFullYear();
        const month = window.myFlightsManager?.currentMonth || (new Date().getMonth() + 1);
        
        switch (this.exportFormat) {
            case 'pdf':
                this.exportToPDF(year, month);
                break;
            case 'excel':
                this.exportToExcel(year, month);
                break;
            case 'json':
                this.exportToJSON(year, month);
                break;
            default:
                this.exportToPDF(year, month);
        }
    }

    exportToPDF(year, month) {
        // Simple print functionality
        const printWindow = window.open('', '_blank');
        const calendarContent = document.getElementById('flightCalendar').outerHTML;
        const monthName = this.getMonthName(month);
        
        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Lịch Chuyến Bay - ${monthName} ${year}</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    .calendar-grid { display: grid; grid-template-columns: repeat(7, 1fr); gap: 1px; background: #ddd; }
                    .calendar-day { background: white; min-height: 100px; padding: 8px; border: 1px solid #ccc; }
                    .calendar-day-header { background: #f0f0f0; padding: 10px; text-align: center; font-weight: bold; }
                    .calendar-day-number { font-weight: bold; margin-bottom: 5px; }
                    .calendar-flight { background: #6366f1; color: white; padding: 2px 6px; margin: 2px 0; font-size: 10px; border-radius: 3px; }
                    .flight-count { background: #6366f1; color: white; font-size: 8px; padding: 2px 6px; border-radius: 10px; float: right; }
                    h1 { text-align: center; color: #6366f1; }
                    @media print {
                        .no-print { display: none; }
                        .calendar-day { break-inside: avoid; }
                    }
                </style>
            </head>
            <body>
                <h1>Lịch Chuyến Bay - ${monthName} ${year}</h1>
                ${calendarContent}
            </body>
            </html>
        `);
        
        printWindow.document.close();
        printWindow.focus();
        
        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 500);
    }

    async exportToExcel(year, month) {
        try {
            const flights = window.myFlightsManager?.flightsData?.all || [];
            const monthFlights = flights.filter(flight => {
                const flightDate = new Date(flight.departureTime);
                return flightDate.getFullYear() === year && (flightDate.getMonth() + 1) === month;
            });

            // Create CSV content
            const csvHeader = 'Ngày,Mã chuyến bay,Từ,Đến,Giờ khởi hành,Giờ đến,Hạng vé,Ghế,Giá vé,Trạng thái\n';
            const csvContent = monthFlights.map(flight => {
                const date = new Date(flight.departureTime).toLocaleDateString('vi-VN');
                const depTime = new Date(flight.departureTime).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
                const arrTime = new Date(flight.arrivalTime).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
                const price = new Intl.NumberFormat('vi-VN').format(flight.totalPrice);
                
                return `"${date}","${flight.flightCode}","${flight.departureAirportCode}","${flight.arrivalAirportCode}","${depTime}","${arrTime}","${flight.className}","${flight.seatNumber || 'N/A'}","${price}","${flight.statusName}"`;
            }).join('\n');

            const fullContent = csvHeader + csvContent;
            
            // Download CSV file
            const blob = new Blob(['\uFEFF' + fullContent], { type: 'text/csv;charset=utf-8;' });
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = `lich-chuyen-bay-${year}-${month.toString().padStart(2, '0')}.csv`;
            link.click();
            
        } catch (error) {
            console.error('Error exporting to Excel:', error);
            alert('Không thể xuất file Excel. Vui lòng thử lại.');
        }
    }

    async exportToJSON(year, month) {
        try {
            const flights = window.myFlightsManager?.flightsData?.all || [];
            const monthFlights = flights.filter(flight => {
                const flightDate = new Date(flight.departureTime);
                return flightDate.getFullYear() === year && (flightDate.getMonth() + 1) === month;
            });

            const exportData = {
                exportDate: new Date().toISOString(),
                period: {
                    year: year,
                    month: month,
                    monthName: this.getMonthName(month)
                },
                totalFlights: monthFlights.length,
                flights: monthFlights
            };

            const jsonContent = JSON.stringify(exportData, null, 2);
            const blob = new Blob([jsonContent], { type: 'application/json' });
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = `flight-data-${year}-${month.toString().padStart(2, '0')}.json`;
            link.click();
            
        } catch (error) {
            console.error('Error exporting to JSON:', error);
            alert('Không thể xuất file JSON. Vui lòng thử lại.');
        }
    }

    // Calendar view switching
    switchView(viewType) {
        this.currentView = viewType;
        // Implement different view types if needed
        console.log('Switched to view:', viewType);
    }

    // Month navigation with keyboard shortcuts
    setupKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            if (e.ctrlKey || e.metaKey) {
                switch (e.code) {
                    case 'ArrowLeft':
                        e.preventDefault();
                        navigateMonth(-1);
                        break;
                    case 'ArrowRight':
                        e.preventDefault();
                        navigateMonth(1);
                        break;
                    case 'KeyT':
                        e.preventDefault();
                        goToToday();
                        break;
                    case 'KeyE':
                        e.preventDefault();
                        this.exportCalendar();
                        break;
                }
            }
        });
    }

    // Touch/swipe support for mobile
    setupTouchSupport() {
        let startX = 0;
        let startY = 0;
        const calendar = document.getElementById('flightCalendar');
        
        if (calendar) {
            calendar.addEventListener('touchstart', (e) => {
                startX = e.touches[0].clientX;
                startY = e.touches[0].clientY;
            });

            calendar.addEventListener('touchend', (e) => {
                if (!startX || !startY) return;

                const endX = e.changedTouches[0].clientX;
                const endY = e.changedTouches[0].clientY;
                
                const diffX = startX - endX;
                const diffY = startY - endY;

                // Only handle horizontal swipes (ignore vertical scrolling)
                if (Math.abs(diffX) > Math.abs(diffY) && Math.abs(diffX) > 50) {
                    if (diffX > 0) {
                        // Swipe left - next month
                        navigateMonth(1);
                    } else {
                        // Swipe right - previous month
                        navigateMonth(-1);
                    }
                }

                startX = 0;
                startY = 0;
            });
        }
    }

    // Enhanced calendar day click handling
    setupCalendarDayClicks() {
        console.log('🔧 Setting up calendar day clicks...');
        document.addEventListener('click', async (e) => {
            console.log('🖱️ Click detected on:', e.target);
            const calendarDay = e.target.closest('.calendar-day');
            if (calendarDay) {
                console.log('📅 Calendar day clicked:', calendarDay);
                if (!e.target.closest('.calendar-flight')) {
                    const dateAttr = calendarDay.getAttribute('data-date');
                    console.log('📊 Date attribute found:', dateAttr);
                    if (dateAttr) {
                        await this.showDayDetailModal(new Date(dateAttr));
                    } else {
                        console.warn('⚠️ No data-date attribute found on calendar day');
                    }
                }
            }
        });
    }

    async showDayDetailModal(date) {
        console.log('🗓️ Calendar day clicked:', date);
        
        // Ensure all flights are loaded first
        await this.ensureFlightsLoaded();
        
        const flights = this.getFlightsForDate(date);
        console.log('✈️ Flights found for date:', flights.length);
        
        // If only one flight, show detail modal directly
        if (flights.length === 1) {
            if (window.myFlightsManager && window.myFlightsManager.showFlightDetail) {
                window.myFlightsManager.showFlightDetail(flights[0].ticketId);
                return;
            }
        }
        
        // Multiple flights or no flights - show list modal
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = 'dayDetailModal';
        modal.innerHTML = `
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header" style="background: linear-gradient(135deg, var(--primary-airline, #1e40af), var(--secondary-airline, #0ea5e9)); color: white; border-radius: 12px 12px 0 0;">
                        <h5 class="modal-title fw-bold">
                            <i class="fas fa-calendar-day me-2"></i>
                            Chuyến bay ngày ${date.toLocaleDateString('vi-VN', { 
                                weekday: 'long',
                                day: '2-digit', 
                                month: '2-digit',
                                year: 'numeric' 
                            })}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0">
                        ${this.generateFlightListContent(flights, date)}
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(modal);
        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
        
        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
        });
    }
    
    generateFlightListContent(flights, date) {
        if (flights.length === 0) {
            return `
                <div class="empty-day-state text-center py-5">
                    <div class="mb-4">
                        <i class="fas fa-calendar-times fa-4x text-muted opacity-25"></i>
                    </div>
                    <h5 class="text-muted mb-2">Không có chuyến bay</h5>
                    <p class="text-muted mb-4">Bạn không có chuyến bay nào trong ngày này</p>
                    <button type="button" class="btn btn-primary btn-sm rounded-pill" data-bs-dismiss="modal">
                        <i class="fas fa-times me-1"></i>
                        Đóng
                    </button>
                </div>
            `;
        }
        
        return `
            <div class="flights-list-container">
                <div class="list-header p-3 bg-light border-bottom">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1 fw-bold text-primary">
                                <i class="fas fa-plane-departure me-2"></i>
                                ${flights.length} chuyến bay
                            </h6>
                            <small class="text-muted">Click vào chuyến bay để xem chi tiết</small>
                        </div>
                        <div class="text-end">
                            <span class="badge bg-primary rounded-pill px-3 py-2">
                                ${date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })}
                            </span>
                        </div>
                    </div>
                </div>
                
                <div class="flights-list p-3">
                    ${flights.map((flight, index) => this.generateFlightListItem(flight, index)).join('')}
                </div>
            </div>
        `;
    }
    
    generateFlightListItem(flight, index) {
        const depTime = new Date(flight.departureTime);
        const arrTime = new Date(flight.arrivalTime);
        const depTimeStr = depTime.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        const arrTimeStr = arrTime.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        
        const duration = this.calculateDuration(depTime, arrTime);
        const totalPrice = this.formatCurrency(flight.totalPrice);
        const statusClass = this.getStatusClass(flight.statusName);
        const statusDisplay = this.getStatusDisplay(flight.statusName);
        const classIcon = this.getClassIcon(flight.className);
        
        return `
            <div class="flight-list-item mb-3 p-3 border rounded-3 position-relative overflow-hidden" 
                 onclick="this.closest('.modal').querySelector('.btn-close').click(); setTimeout(() => window.myFlightsManager.showFlightDetail(${flight.ticketId}), 300);" 
                 style="cursor: pointer; transition: all 0.3s ease; border-left: 4px solid var(--primary-airline, #1e40af) !important;">
                
                <!-- Flight Header -->
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div>
                        <h6 class="mb-1 fw-bold text-primary" style="font-family: 'Monaco', 'Consolas', monospace;">
                            ${this.formatFlightCode(flight.flightCode)}
                        </h6>
                        <div class="text-muted small">
                            <i class="fas fa-ticket-alt me-1"></i>
                            ${flight.ticketNumber}
                        </div>
                    </div>
                    <span class="badge flight-status-${statusClass} px-3 py-2 rounded-pill">
                        ${statusDisplay}
                    </span>
                </div>
                
                <!-- Route Information -->
                <div class="flight-route-compact mb-3">
                    <div class="row align-items-center">
                        <div class="col-4 text-start">
                            <div class="airport-info">
                                <div class="airport-code h5 mb-1 fw-bold text-primary">${flight.departureAirportCode}</div>
                                <div class="airport-name small text-muted">${this.truncateText(flight.departureAirportName, 15)}</div>
                                <div class="time fw-semibold">${depTimeStr}</div>
                            </div>
                        </div>
                        <div class="col-4 text-center">
                            <div class="flight-path">
                                <i class="fas fa-plane text-primary mb-1"></i>
                                <div class="duration-line position-relative">
                                    <div class="border-top border-2 border-primary opacity-25"></div>
                                    <small class="duration-text bg-white px-2 text-muted">${duration}</small>
                                </div>
                            </div>
                        </div>
                        <div class="col-4 text-end">
                            <div class="airport-info">
                                <div class="airport-code h5 mb-1 fw-bold text-primary">${flight.arrivalAirportCode}</div>
                                <div class="airport-name small text-muted">${this.truncateText(flight.arrivalAirportName, 15)}</div>
                                <div class="time fw-semibold">${arrTimeStr}</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Flight Details Footer -->
                <div class="flight-details-footer">
                    <div class="row align-items-center">
                        <div class="col-6">
                            <div class="d-flex align-items-center">
                                <span class="me-2">${classIcon}</span>
                                <span class="fw-semibold">${flight.className}</span>
                                ${flight.seatNumber ? `<span class="ms-2 badge bg-secondary">${flight.seatNumber}</span>` : ''}
                            </div>
                        </div>
                        <div class="col-6 text-end">
                            <div class="price h6 mb-0 fw-bold text-success">${totalPrice}</div>
                        </div>
                    </div>
                </div>
                
                <!-- Hover effect overlay -->
                <div class="hover-overlay position-absolute top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" 
                     style="background: rgba(30, 64, 175, 0.1); opacity: 0; transition: opacity 0.3s ease; pointer-events: none;">
                    <span class="text-primary fw-bold">
                        <i class="fas fa-eye me-2"></i>
                        Xem chi tiết
                    </span>
                </div>
            </div>
        `;
    }
    
    // Helper methods for the calendar
    calculateDuration(depTime, arrTime) {
        const diff = arrTime - depTime;
        const hours = Math.floor(diff / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        return `${hours}h ${minutes}m`;
    }
    
    formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
            minimumFractionDigits: 0
        }).format(amount);
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
        if (classLower.includes('first') || classLower.includes('elite')) return '<i class="fas fa-crown text-warning"></i>';
        if (classLower.includes('business') || classLower.includes('premium')) return '<i class="fas fa-star text-info"></i>';
        return '<i class="fas fa-chair text-primary"></i>';
    }
    
    formatFlightCode(code) {
        if (code && code.length > 8) {
            return code.substring(0, 4) + ' ' + code.substring(4, 8) + ' ' + code.substring(8);
        }
        return code;
    }
    
    truncateText(text, maxLength) {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    }

    async ensureFlightsLoaded() {
        if (!window.myFlightsManager) {
            console.warn('⚠️ MyFlightsManager not found');
            return;
        }
        
        const manager = window.myFlightsManager;
        
        // Check if all flights data is already loaded
        if (manager.flightsData?.allLoaded && manager.flightsData.all?.length > 0) {
            console.log('✅ All flights already loaded');
            return;
        }
        
        // Load all flights if not loaded yet
        if (!manager.flightsData?.allLoaded) {
            console.log('🔄 Loading all flights for calendar...');
            try {
                await manager.loadAllFlights();
                console.log('✅ All flights loaded successfully');
            } catch (error) {
                console.error('❌ Error loading all flights:', error);
            }
        }
    }

    getFlightsForDate(date) {
        // Try to get flights from multiple sources
        let flights = [];
        
        if (window.myFlightsManager?.flightsData) {
            const flightData = window.myFlightsManager.flightsData;
            // Combine upcoming, completed, and all flights
            flights = [
                ...(flightData.upcoming || []),
                ...(flightData.completed || []),
                ...(flightData.all || [])
            ];
            
            // Remove duplicates based on ticketId
            flights = flights.filter((flight, index, self) => 
                index === self.findIndex(f => f.ticketId === flight.ticketId)
            );
        }
        
        return flights.filter(flight => {
            const flightDate = new Date(flight.departureTime);
            return flightDate.toDateString() === date.toDateString();
        });
    }

    getMonthName(month) {
        const months = [
            'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
            'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
        ];
        return months[month - 1];
    }

    // Calendar accessibility improvements
    setupAccessibility() {
        // Add ARIA labels and keyboard navigation
        const calendarDays = document.querySelectorAll('.calendar-day');
        calendarDays.forEach((day, index) => {
            day.setAttribute('role', 'gridcell');
            day.setAttribute('tabindex', '-1');
            day.setAttribute('aria-label', this.getAriaLabel(day));
        });

        // Make first day focusable
        if (calendarDays.length > 0) {
            calendarDays[0].setAttribute('tabindex', '0');
        }

        // Arrow key navigation within calendar
        document.addEventListener('keydown', (e) => {
            const focusedDay = document.activeElement;
            if (focusedDay && focusedDay.classList.contains('calendar-day')) {
                const days = Array.from(calendarDays);
                const currentIndex = days.indexOf(focusedDay);
                let newIndex = currentIndex;

                switch (e.code) {
                    case 'ArrowRight':
                        e.preventDefault();
                        newIndex = Math.min(currentIndex + 1, days.length - 1);
                        break;
                    case 'ArrowLeft':
                        e.preventDefault();
                        newIndex = Math.max(currentIndex - 1, 0);
                        break;
                    case 'ArrowDown':
                        e.preventDefault();
                        newIndex = Math.min(currentIndex + 7, days.length - 1);
                        break;
                    case 'ArrowUp':
                        e.preventDefault();
                        newIndex = Math.max(currentIndex - 7, 0);
                        break;
                    case 'Enter':
                    case 'Space':
                        e.preventDefault();
                        focusedDay.click();
                        break;
                }

                if (newIndex !== currentIndex) {
                    days[currentIndex].setAttribute('tabindex', '-1');
                    days[newIndex].setAttribute('tabindex', '0');
                    days[newIndex].focus();
                }
            }
        });
    }

    getAriaLabel(dayElement) {
        const dateAttr = dayElement.getAttribute('data-date');
        if (!dateAttr) return '';

        const date = new Date(dateAttr);
        const flights = this.getFlightsForDate(date);
        const dayText = date.toLocaleDateString('vi-VN', { weekday: 'long', day: 'numeric', month: 'long' });
        
        if (flights.length === 0) {
            return `${dayText}, không có chuyến bay`;
        } else if (flights.length === 1) {
            return `${dayText}, có 1 chuyến bay`;
        } else {
            return `${dayText}, có ${flights.length} chuyến bay`;
        }
    }
}

// Global functions for calendar extensions
function exportCalendar() {
    if (window.calendarExtensions) {
        window.calendarExtensions.exportCalendar();
    }
}

function printFlightDetail() {
    const modalContent = document.getElementById('flightDetailContent');
    if (modalContent) {
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Chi tiết chuyến bay</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    .flight-detail-content { max-width: 800px; margin: 0 auto; }
                    .route-timeline { display: flex; justify-content: space-between; align-items: center; margin: 20px 0; }
                    .route-point { text-align: center; }
                    .route-time { font-size: 18px; font-weight: bold; color: #6366f1; }
                    .route-airport strong { font-size: 16px; display: block; margin: 5px 0; }
                    .route-line { flex: 1; height: 2px; background: #6366f1; margin: 0 20px; position: relative; }
                    .route-line i { position: absolute; top: -8px; left: 50%; transform: translateX(-50%); background: white; color: #6366f1; }
                    .passenger-info, .ticket-summary { background: #f8f9fa; padding: 15px; margin: 15px 0; border-radius: 5px; }
                    .summary-item { display: flex; justify-content: space-between; padding: 5px 0; border-bottom: 1px solid #ddd; }
                    @media print { .no-print { display: none; } }
                </style>
            </head>
            <body>
                <h1>Chi tiết chuyến bay</h1>
                ${modalContent.innerHTML}
            </body>
            </html>
        `);
        
        printWindow.document.close();
        printWindow.focus();
        
        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 500);
    }
}

// Initialize calendar extensions when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.calendarExtensions = new CalendarExtensions();
    
    // Setup all extensions
    window.calendarExtensions.setupKeyboardShortcuts();
    window.calendarExtensions.setupTouchSupport();
    window.calendarExtensions.setupCalendarDayClicks();
    
    // Setup accessibility after calendar is rendered
    setTimeout(() => {
        window.calendarExtensions.setupAccessibility();
    }, 1000);
});
