// Item Management JavaScript

let currentPage = 1;
let pageSize = 10;
let searchTerm = '';
let statusFilter = '';

document.addEventListener('DOMContentLoaded', function() {
    loadItems();
    setupEventListeners();
});

function setupEventListeners() {
    // Search input
    document.getElementById('searchTerm').addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            searchItems();
        }
    });

    // Status filter change
    document.getElementById('statusFilter').addEventListener('change', function() {
        searchItems();
    });
}

function getAuthToken() {
    return localStorage.getItem('token') || sessionStorage.getItem('token');
}

function getAuthHeaders() {
    const token = getAuthToken();
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };
}

function searchItems() {
    searchTerm = document.getElementById('searchTerm').value.trim();
    statusFilter = document.getElementById('statusFilter').value;
    currentPage = 1;
    loadItems();
}

function resetFilters() {
    document.getElementById('searchTerm').value = '';
    document.getElementById('statusFilter').value = '';
    searchTerm = '';
    statusFilter = '';
    currentPage = 1;
    loadItems();
}

async function loadItems(page = 1) {
    try {
        showLoading();

        const requestData = {
            search: searchTerm,
            statusId: statusFilter ? parseInt(statusFilter) : null,
            page: page,
            pageSize: pageSize
        };

        const response = await fetch('/ItemManage/GetItems', {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(requestData)
        });

        const result = await response.json();

        if (result.success) {
            currentPage = page;
            displayItems(result.data);
            displayPagination(result.pagination);
        } else {
            showAlert('error', result.message || 'Failed to load items');
        }
    } catch (error) {
        console.error('Error loading items:', error);
        showAlert('error', 'Error loading items: ' + error.message);
    } finally {
        hideLoading();
    }
}

function displayItems(items) {
    const tbody = document.getElementById('itemsTableBody');
    
    if (!items || items.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    <div class="empty-state">
                        <i class="fas fa-box-open"></i>
                        <h3>No items found</h3>
                        <p>Try adjusting your search criteria or add a new item</p>
                    </div>
                </td>
            </tr>
        `;
        updateStats(0, 0, 0);
        return;
    }

    tbody.innerHTML = items.map(item => `
        <tr class="fade-in">
            <td>
                <span class="fw-bold text-primary">#${item.itemId}</span>
            </td>
            <td>
                <div class="item-image-container">
                    <img src="${item.image || '/images/default-item.jpg'}" 
                         alt="${item.itemName}" 
                         class="item-image"
                         onerror="this.src='/images/default-item.jpg'">
                    <div class="image-overlay">
                        <i class="fas fa-eye"></i>
                    </div>
                </div>
            </td>
            <td>
                <div class="fw-bold text-dark">${escapeHtml(item.itemName)}</div>
                <small class="text-muted">Item ID: ${item.itemId}</small>
            </td>
            <td>
                <div class="text-truncate" style="max-width: 200px;" title="${escapeHtml(item.detail || '')}">
                    ${escapeHtml(item.detail || 'No description')}
                </div>
            </td>
            <td>
                <span class="fw-bold text-success price">${formatPrice(item.price)}</span>
            </td>
            <td>
                <span class="status-badge ${getStatusClass(item.statusId)}">
                    <i class="fas ${item.statusId === 1 ? 'fa-check-circle' : 'fa-times-circle'}"></i>
                    ${getStatusText(item.statusId)}
                </span>
            </td>
            <td>
                <div class="action-buttons">
                    <a href="/ItemManage/ItemDetails/${item.itemId}" 
                       class="btn btn-info action-btn" 
                       title="View Details">
                        <i class="fas fa-eye"></i>
                    </a>
                    <button class="btn btn-warning action-btn" 
                            onclick="editItem(${item.itemId})" 
                            title="Edit Item">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-danger action-btn" 
                            onclick="deleteItem(${item.itemId})" 
                            title="Delete Item">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');

    // Update stats
    const total = items.length;
    const active = items.filter(item => item.statusId === 1).length;
    const inactive = items.filter(item => item.statusId === 2).length;
    updateStats(total, active, inactive);
}

function updateStats(total, active, inactive) {
    document.getElementById('totalItems').textContent = total;
    document.getElementById('activeItems').textContent = active;
    document.getElementById('inactiveItems').textContent = inactive;
}

function displayPagination(pagination) {
    const paginationContainer = document.getElementById('pagination');
    
    if (!pagination || pagination.totalPages <= 1) {
        paginationContainer.innerHTML = '';
        return;
    }

    const { currentPage, totalPages } = pagination;
    let paginationHtml = '';

    // Previous button
    paginationHtml += `
        <li class="page-item ${currentPage <= 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadItems(${currentPage - 1}); return false;">
                <i class="fas fa-chevron-left"></i> Previous
            </a>
        </li>
    `;

    // Page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    if (startPage > 1) {
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="loadItems(1); return false;">1</a>
            </li>
        `;
        if (startPage > 2) {
            paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        paginationHtml += `
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="loadItems(${i}); return false;">${i}</a>
            </li>
        `;
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="loadItems(${totalPages}); return false;">${totalPages}</a>
            </li>
        `;
    }

    // Next button
    paginationHtml += `
        <li class="page-item ${currentPage >= totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadItems(${currentPage + 1}); return false;">
                Next <i class="fas fa-chevron-right"></i>
            </a>
        </li>
    `;

    paginationContainer.innerHTML = paginationHtml;
}

async function editItem(itemId) {
    try {
        const response = await fetch(`/ItemManage/GetItem?id=${itemId}`, {
            headers: getAuthHeaders()
        });

        const result = await response.json();

        if (result.success && result.data) {
            // Redirect to ItemDetails page which has edit functionality
            window.location.href = `/ItemManage/ItemDetails/${itemId}`;
        } else {
            showAlert('error', result.message || 'Item not found');
        }
    } catch (error) {
        console.error('Error loading item:', error);
        showAlert('error', 'Error loading item: ' + error.message);
    }
}

let itemToDelete = null;

function deleteItem(itemId) {
    itemToDelete = itemId;
    
    if (confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
        confirmDelete();
    }
}

async function confirmDelete() {
    if (!itemToDelete) return;

    try {
        const response = await fetch(`/ItemManage/DeleteItem?id=${itemToDelete}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        const result = await response.json();

        if (result.success) {
            showAlert('success', 'Item deleted successfully');
            loadItems(currentPage);
        } else {
            showAlert('error', result.message || 'Failed to delete item');
        }
    } catch (error) {
        console.error('Error deleting item:', error);
        showAlert('error', 'Error deleting item: ' + error.message);
    } finally {
        itemToDelete = null;
    }
}

function viewItem(itemId) {
    window.location.href = `/ItemManage/ItemDetails/${itemId}`;
}

function showLoading() {
    const tbody = document.getElementById('itemsTableBody');
    tbody.innerHTML = `
        <tr>
            <td colspan="7" class="text-center">
                <div class="loading-container">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="mt-2 text-muted">Loading items...</p>
                </div>
            </td>
        </tr>
    `;
}

function hideLoading() {
    // Loading will be replaced by actual content
}

function getStatusClass(statusId) {
    switch(statusId) {
        case 1:
            return 'status-active';
        case 2:
            return 'status-inactive';
        default:
            return 'badge-secondary';
    }
}

function getStatusText(statusId) {
    switch(statusId) {
        case 1:
            return 'Active';
        case 2:
            return 'Inactive';
        default:
            return 'Unknown';
    }
}

function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function showAlert(type, message) {
    const alertContainer = document.getElementById('alertContainer') || createAlertContainer();
    
    const alertClass = type === 'success' ? 'alert-success' : 
                     type === 'error' ? 'alert-danger' : 
                     type === 'warning' ? 'alert-warning' : 'alert-info';
    
    const iconClass = type === 'success' ? 'check-circle' : 
                     type === 'error' ? 'exclamation-triangle' : 
                     type === 'warning' ? 'exclamation-triangle' : 'info-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas fa-${iconClass}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    alertContainer.innerHTML = alertHtml;
    
    // Auto dismiss after 5 seconds for non-error messages
    if (type !== 'error') {
        setTimeout(() => {
            const alert = alertContainer.querySelector('.alert');
            if (alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }
        }, 5000);
    }
}

function createAlertContainer() {
    const container = document.createElement('div');
    container.id = 'alertContainer';
    container.style.position = 'fixed';
    container.style.top = '20px';
    container.style.right = '20px';
    container.style.zIndex = '9999';
    container.style.maxWidth = '400px';
    document.body.appendChild(container);
    return container;
}
