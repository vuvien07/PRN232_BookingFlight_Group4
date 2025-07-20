// Ticket Management JavaScript
let allTickets = [];
let filteredTickets = [];
let currentPage = 1;
let pageSize = 10;
let totalPages = 1;
let totalCount = 0;

// Initialize page when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    loadAllTickets();
    setupEventListeners();
});

// Setup event listeners
function setupEventListeners() {
    // Search input with debounce
    let searchTimeout;
    document.getElementById('searchInput').addEventListener('input', function() {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(searchTickets, 300);
    });
}

// Load all tickets from API with pagination
async function loadAllTickets(page = 1) {
    try {
        showLoadingIndicator();
        hideNoDataMessage();
        
        const response = await fetch(`/Manager/GetTicketsPaginated?page=${page}&pageSize=${pageSize}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const paginatedResult = await response.json();
            allTickets = paginatedResult.tickets;
            filteredTickets = [...allTickets];
            currentPage = paginatedResult.page;
            totalPages = paginatedResult.totalPages;
            totalCount = paginatedResult.totalCount;
            
            displayTickets(filteredTickets);
            updateStatistics();
            updatePaginationControls();
        } else {
            throw new Error('Failed to load tickets');
        }
    } catch (error) {
        console.error('Error loading tickets:', error);
        showErrorMessage('Failed to load tickets. Please try again.');
    } finally {
        hideLoadingIndicator();
    }
}

// Display tickets in table
function displayTickets(tickets) {
    const tableBody = document.getElementById('ticketsTableBody');
    
    if (!tickets || tickets.length === 0) {
        tableBody.innerHTML = '';
        showNoDataMessage();
        return;
    }
    
    hideNoDataMessage();
    
    const ticketsHtml = tickets.map(ticket => `
        <tr>
            <td>
                <span class="flight-code">${ticket.ticketNumber || 'N/A'}</span>
            </td>
            <td>
                <div>
                    <strong>${ticket.fullName || ticket.name || 'N/A'}</strong>
                    <br>
                    <small class="text-muted">${ticket.gender || ''} • ${formatDate(ticket.dateOfBirth) || ''}</small>
                </div>
            </td>
            <td>
                <span class="flight-code">${ticket.flightDTO?.flightCode || 'N/A'}</span>
            </td>
            <td>
                <div class="route-display">
                    ${ticket.flightDTO?.departureAirport?.airportName || 'N/A'}
                    <span class="route-arrow">→</span>
                    ${ticket.flightDTO?.arrivalAirport?.airportName || 'N/A'}
                </div>
            </td>
            <td>
                <span class="badge bg-info">${ticket.classSeatDTO?.className || 'N/A'}</span>
            </td>
            <td>
                <span class="price-display">$${formatPrice(ticket.totalPrice)}</span>
            </td>
            <td>
                ${formatDate(ticket.bookingDate)}
            </td>
            <td>
                <span class="status-badge ${getStatusClass(ticket.statusId)}">
                    ${getStatusText(ticket.statusId)}
                </span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="btn action-btn view" onclick="viewTicketDetails(${ticket.ticketId})" title="View Details">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn action-btn edit" onclick="openUpdateStatusModal(${ticket.ticketId}, ${ticket.statusId})" title="Update Status">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn action-btn delete" onclick="confirmDeleteTicket(${ticket.ticketId})" title="Delete Ticket">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
    
    tableBody.innerHTML = ticketsHtml;
}

// Update statistics
function updateStatistics() {
    const total = filteredTickets.length;
    const active = filteredTickets.filter(t => t.statusId === 1).length; // Active tickets
    const inactive = filteredTickets.filter(t => t.statusId === 2).length; // Inactive tickets
    
    document.getElementById('totalTickets').textContent = total;
    document.getElementById('confirmedTickets').textContent = active; // Reuse confirmed for active
    document.getElementById('pendingTickets').textContent = inactive; // Reuse pending for inactive  
    document.getElementById('cancelledTickets').textContent = 0; // Always 0 since we only have 2 statuses
}

// Apply filters
function applyFilters() {
    let filtered = [...allTickets];
    
    // Status filter
    const statusFilter = document.getElementById('statusFilter').value;
    if (statusFilter) {
        filtered = filtered.filter(ticket => ticket.statusId === parseInt(statusFilter));
    }
    
    // Date range filter
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;
    
    if (startDate || endDate) {
        filtered = filtered.filter(ticket => {
            const bookingDate = new Date(ticket.bookingDate);
            if (startDate && bookingDate < new Date(startDate)) return false;
            if (endDate && bookingDate > new Date(endDate)) return false;
            return true;
        });
    }
    
    filteredTickets = filtered;
    searchTickets(); // Apply search to filtered results
    updateStatistics();
}

// Search tickets
function searchTickets() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase().trim();
    
    if (!searchTerm) {
        displayTickets(filteredTickets);
        return;
    }
    
    const searchResults = filteredTickets.filter(ticket => {
        return (
            (ticket.ticketNumber && ticket.ticketNumber.toLowerCase().includes(searchTerm)) ||
            (ticket.fullName && ticket.fullName.toLowerCase().includes(searchTerm)) ||
            (ticket.name && ticket.name.toLowerCase().includes(searchTerm)) ||
            (ticket.flightDTO?.flightCode && ticket.flightDTO.flightCode.toLowerCase().includes(searchTerm)) ||
            (ticket.contactEmail && ticket.contactEmail.toLowerCase().includes(searchTerm))
        );
    });
    
    displayTickets(searchResults);
}

// Clear search
function clearSearch() {
    document.getElementById('searchInput').value = '';
    searchTickets();
}

// Refresh tickets
function refreshTickets() {
    // Clear filters
    document.getElementById('statusFilter').value = '';
    document.getElementById('startDate').value = '';
    document.getElementById('endDate').value = '';
    document.getElementById('searchInput').value = '';
    
    // Reload data
    loadAllTickets();
}

// View ticket details
async function viewTicketDetails(ticketId) {
    try {
        const ticket = allTickets.find(t => t.ticketId === ticketId);
        if (!ticket) {
            showErrorMessage('Ticket not found');
            return;
        }
        
        const detailsHtml = `
            <div class="ticket-details">
                <div class="row">
                    <div class="col-md-6">
                        <h6>Ticket Information</h6>
                        <p><strong>Ticket Number:</strong> ${ticket.ticketNumber || 'N/A'}</p>
                        <p><strong>Booking Date:</strong> ${formatDate(ticket.bookingDate)}</p>
                        <p><strong>Total Price:</strong> $${formatPrice(ticket.totalPrice)}</p>
                        <p><strong>Status:</strong> <span class="status-badge ${getStatusClass(ticket.statusId)}">${getStatusText(ticket.statusId)}</span></p>
                    </div>
                    <div class="col-md-6">
                        <h6>Flight Information</h6>
                        <p><strong>Flight Code:</strong> ${ticket.flightDTO?.flightCode || 'N/A'}</p>
                        <p><strong>Route:</strong> ${ticket.flightDTO?.departureAirport?.airportName || 'N/A'} → ${ticket.flightDTO?.arrivalAirport?.airportName || 'N/A'}</p>
                        <p><strong>Departure:</strong> ${formatDateTime(ticket.flightDTO?.departureTime)}</p>
                        <p><strong>Arrival:</strong> ${formatDateTime(ticket.flightDTO?.arrivalTime)}</p>
                    </div>
                </div>
                <div class="row mt-3">
                    <div class="col-md-6">
                        <h6>Passenger Information</h6>
                        <p><strong>Name:</strong> ${ticket.fullName || ticket.name || 'N/A'}</p>
                        <p><strong>Gender:</strong> ${ticket.gender || 'N/A'}</p>
                        <p><strong>Date of Birth:</strong> ${formatDate(ticket.dateOfBirth)}</p>
                        <p><strong>Class:</strong> ${ticket.classSeatDTO?.className || 'N/A'}</p>
                    </div>
                    <div class="col-md-6">
                        <h6>Contact Information</h6>
                        <p><strong>Contact Name:</strong> ${ticket.contactFullName || 'N/A'}</p>
                        <p><strong>Phone:</strong> ${ticket.contactPhone || 'N/A'}</p>
                        <p><strong>Email:</strong> ${ticket.contactEmail || 'N/A'}</p>
                        <p><strong>Address:</strong> ${ticket.contactAddress || 'N/A'}</p>
                    </div>
                </div>
            </div>
        `;
        
        document.getElementById('ticketDetailsContent').innerHTML = detailsHtml;
        
        const modal = new bootstrap.Modal(document.getElementById('ticketDetailsModal'));
        modal.show();
        
    } catch (error) {
        console.error('Error viewing ticket details:', error);
        showErrorMessage('Failed to load ticket details');
    }
}

// Open update status modal
function openUpdateStatusModal(ticketId, currentStatusId) {
    document.getElementById('ticketIdToUpdate').value = ticketId;
    document.getElementById('newStatus').value = currentStatusId;
    
    const modal = new bootstrap.Modal(document.getElementById('updateStatusModal'));
    modal.show();
}

// Update ticket status
async function updateTicketStatus() {
    try {
        const ticketId = document.getElementById('ticketIdToUpdate').value;
        const newStatusId = document.getElementById('newStatus').value;
        
        if (!ticketId || !newStatusId) {
            showErrorMessage('Please select a valid status');
            return;
        }
        
        const response = await fetch(`/Manager/UpdateTicketStatus?ticketId=${ticketId}&statusId=${newStatusId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            showSuccessMessage('Ticket status updated successfully');
            
            // Update local data
            const ticketIndex = allTickets.findIndex(t => t.ticketId === parseInt(ticketId));
            if (ticketIndex !== -1) {
                allTickets[ticketIndex].statusId = parseInt(newStatusId);
            }
            
            applyFilters(); // Refresh display
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('updateStatusModal'));
            modal.hide();
        } else {
            throw new Error('Failed to update ticket status');
        }
    } catch (error) {
        console.error('Error updating ticket status:', error);
        showErrorMessage('Failed to update ticket status');
    }
}

// Confirm delete ticket
function confirmDeleteTicket(ticketId) {
    if (confirm('Are you sure you want to delete this ticket? This action cannot be undone.')) {
        deleteTicket(ticketId);
    }
}

// Delete ticket
async function deleteTicket(ticketId) {
    try {
        const response = await fetch(`/Manager/DeleteTicket/${ticketId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            showSuccessMessage('Ticket deleted successfully');
            
            // Remove from local data
            allTickets = allTickets.filter(t => t.ticketId !== ticketId);
            applyFilters(); // Refresh display
        } else {
            throw new Error('Failed to delete ticket');
        }
    } catch (error) {
        console.error('Error deleting ticket:', error);
        showErrorMessage('Failed to delete ticket');
    }
}

// Utility functions
function formatDate(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

function formatDateTime(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function formatPrice(price) {
    if (!price) return '0.00';
    return parseFloat(price).toFixed(2);
}

function getStatusClass(statusId) {
    switch (statusId) {
        case 1: return 'status-confirmed'; // Active
        case 2: return 'status-cancelled'; // Inactive
        default: return 'status-pending';
    }
}

function getStatusText(statusId) {
    switch (statusId) {
        case 1: return 'Active';
        case 2: return 'Inactive';
        default: return 'Unknown';
    }
}

// UI state management
function showLoadingIndicator() {
    document.getElementById('loadingIndicator').style.display = 'block';
}

function hideLoadingIndicator() {
    document.getElementById('loadingIndicator').style.display = 'none';
}

function showNoDataMessage() {
    document.getElementById('noDataMessage').style.display = 'block';
}

function hideNoDataMessage() {
    document.getElementById('noDataMessage').style.display = 'none';
}

function showSuccessMessage(message) {
    // You can implement toast notifications here
    alert(message); // Simple alert for now
}

// Pagination functions
function updatePaginationControls() {
    const paginationContainer = document.getElementById('paginationControls');
    if (!paginationContainer) return;
    
    let paginationHTML = '';
    
    if (totalPages > 1) {
        paginationHTML += '<nav aria-label="Page navigation">';
        paginationHTML += '<ul class="pagination justify-content-center">';
        
        // Previous button
        if (currentPage > 1) {
            paginationHTML += `<li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${currentPage - 1})">Previous</a>
            </li>`;
        } else {
            paginationHTML += '<li class="page-item disabled"><span class="page-link">Previous</span></li>';
        }
        
        // Page numbers
        for (let i = Math.max(1, currentPage - 2); i <= Math.min(totalPages, currentPage + 2); i++) {
            if (i === currentPage) {
                paginationHTML += `<li class="page-item active">
                    <span class="page-link">${i}</span>
                </li>`;
            } else {
                paginationHTML += `<li class="page-item">
                    <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                </li>`;
            }
        }
        
        // Next button
        if (currentPage < totalPages) {
            paginationHTML += `<li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${currentPage + 1})">Next</a>
            </li>`;
        } else {
            paginationHTML += '<li class="page-item disabled"><span class="page-link">Next</span></li>';
        }
        
        paginationHTML += '</ul>';
        paginationHTML += '</nav>';
        
        // Page info
        const startItem = (currentPage - 1) * pageSize + 1;
        const endItem = Math.min(currentPage * pageSize, totalCount);
        paginationHTML += `<div class="text-center mt-2">
            <small class="text-muted">Showing ${startItem} to ${endItem} of ${totalCount} tickets</small>
        </div>`;
    }
    
    paginationContainer.innerHTML = paginationHTML;
}

function changePage(page) {
    if (page >= 1 && page <= totalPages && page !== currentPage) {
        loadAllTickets(page);
    }
}

function showErrorMessage(message) {
    // You can implement toast notifications here
    alert(message); // Simple alert for now
}
