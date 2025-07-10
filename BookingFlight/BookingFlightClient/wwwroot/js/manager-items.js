let currentPage = 1;
let pageSize = 10;
let totalPages = 1;
let currentFilters = {
    searchTerm: '',
    statusId: null,
    priceRange: null
};
let allStatuses = [];
let currentEditingItem = null;

document.addEventListener('DOMContentLoaded', function() {
    loadStatuses();
    loadItems();
    setupEventListeners();
});

function setupEventListeners() {
    // Search input with debounce
    let searchTimeout;
    document.getElementById('searchInput').addEventListener('input', function() {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            applyFilters();
        }, 500);
    });

    // Filter change events
    document.getElementById('statusFilter').addEventListener('change', applyFilters);
    document.getElementById('priceRangeFilter').addEventListener('change', applyFilters);

    // Create item form preview
    setupCreateItemPreview();
    setupEditItemPreview();
}

function setupCreateItemPreview() {
    const inputs = ['itemName', 'itemDetail', 'itemPrice', 'itemStatus'];
    inputs.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('input', updateCreateItemPreview);
        }
    });
    updateCreateItemPreview();
}

function setupEditItemPreview() {
    const inputs = ['editItemName', 'editItemDetail', 'editItemPrice', 'editItemStatus'];
    inputs.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('input', updateEditItemPreview);
        }
    });
}

function updateCreateItemPreview() {
    const name = document.getElementById('itemName').value || 'Tên Item';
    const detail = document.getElementById('itemDetail').value || 'Mô tả chi tiết...';
    const price = document.getElementById('itemPrice').value || '0';
    const statusId = document.getElementById('itemStatus').value;

    document.querySelector('.item-preview-name').textContent = name;
    document.querySelector('.item-preview-detail').textContent = detail;
    document.querySelector('.item-preview-price').textContent = formatCurrency(parseInt(price) || 0);
    
    const status = allStatuses.find(s => s.statusId == statusId);
    const statusElement = document.querySelector('.item-preview-status');
    if (status) {
        statusElement.textContent = status.statusName;
        statusElement.className = `badge ${status.statusId == 1 ? 'bg-success' : 'bg-secondary'}`;
    } else {
        statusElement.textContent = 'Active';
        statusElement.className = 'badge bg-success';
    }
}

function updateEditItemPreview() {
    const name = document.getElementById('editItemName').value || 'Tên Item';
    const detail = document.getElementById('editItemDetail').value || 'Mô tả chi tiết...';
    const price = document.getElementById('editItemPrice').value || '0';
    const statusId = document.getElementById('editItemStatus').value;

    document.querySelector('.edit-item-preview-name').textContent = name;
    document.querySelector('.edit-item-preview-detail').textContent = detail;
    document.querySelector('.edit-item-preview-price').textContent = formatCurrency(parseInt(price) || 0);
    
    const status = allStatuses.find(s => s.statusId == statusId);
    const statusElement = document.querySelector('.edit-item-preview-status');
    if (status) {
        statusElement.textContent = status.statusName;
        statusElement.className = `badge ${status.statusId == 1 ? 'bg-success' : 'bg-secondary'}`;
    } else {
        statusElement.textContent = 'Active';
        statusElement.className = 'badge bg-success';
    }
}

async function loadStatuses() {
    try {
        const response = await fetch('/api/manager/items/statuses');
        const result = await response.json();
        
        if (result.success) {
            allStatuses = result.data;
            populateStatusFilters();
        }
    } catch (error) {
        console.error('Error loading statuses:', error);
    }
}

function populateStatusFilters() {
    const statusFilter = document.getElementById('statusFilter');
    const itemStatus = document.getElementById('itemStatus');
    const editItemStatus = document.getElementById('editItemStatus');
    
    allStatuses.forEach(status => {
        if (statusFilter) {
            const option = document.createElement('option');
            option.value = status.statusId;
            option.textContent = status.statusName;
            statusFilter.appendChild(option);
        }
        
        if (itemStatus) {
            const option = document.createElement('option');
            option.value = status.statusId;
            option.textContent = status.statusName;
            itemStatus.appendChild(option);
        }
        
        if (editItemStatus) {
            const option = document.createElement('option');
            option.value = status.statusId;
            option.textContent = status.statusName;
            editItemStatus.appendChild(option);
        }
    });
}

function applyFilters() {
    currentFilters.searchTerm = document.getElementById('searchInput').value;
    currentFilters.statusId = document.getElementById('statusFilter').value ? parseInt(document.getElementById('statusFilter').value) : null;
    currentFilters.priceRange = document.getElementById('priceRangeFilter').value;
    currentPage = 1;
    loadItems();
}

async function loadItems() {
    showLoading(true);
    hideMessages();

    const requestData = {
        page: currentPage,
        pageSize: pageSize,
        searchTerm: currentFilters.searchTerm || null,
        statusId: currentFilters.statusId || null
    };

    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');
        
        const response = await fetch('/api/manager/items/list', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify(requestData)
        });

        const result = await response.json();

        if (result.success) {
            displayItems(result.data);
            updatePagination(result.pagination);
            document.getElementById('itemsCount').textContent = `${result.pagination.totalCount} items`;
        } else {
            showError(result.message || 'Không thể tải danh sách items');
            showNoData();
        }
    } catch (error) {
        console.error('Error loading items:', error);
        showError('Lỗi khi tải danh sách items');
        showNoData();
    } finally {
        showLoading(false);
    }
}

function displayItems(items) {
    const container = document.getElementById('itemsTableBody');
    
    if (!items || items.length === 0) {
        showNoData();
        return;
    }

    container.innerHTML = items.map(item => createItemRow(item)).join('');
    document.getElementById('itemsTable').style.display = 'table';
    document.getElementById('noDataMessage').style.display = 'none';
}

function createItemRow(item) {
    const statusClass = item.statusId === 1 ? 'bg-success' : 'bg-secondary';
    const statusText = item.statusName || 'Unknown';
    
    return `
        <tr>
            <td>${item.itemId}</td>
            <td>
                <div class="d-flex align-items-center">
                    <img src="${item.image || '/images/default.jpg'}" alt="${item.itemName}" 
                         class="item-thumbnail me-2" style="width: 40px; height: 40px; object-fit: cover; border-radius: 4px;">
                    <span>${escapeHtml(item.itemName)}</span>
                </div>
            </td>
            <td>${escapeHtml(item.detail || '')}</td>
            <td>${formatCurrency(item.price)}</td>
            <td><span class="badge ${statusClass}">${statusText}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-info" onclick="viewItemDetails(${item.itemId})" title="Xem chi tiết">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn btn-outline-primary" onclick="editItem(${item.itemId})" title="Chỉnh sửa">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-outline-danger" onclick="deleteItem(${item.itemId})" title="Xóa">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `;
}

function showNoData() {
    document.getElementById('itemsTable').style.display = 'none';
    document.getElementById('noDataMessage').style.display = 'block';
}

function showLoading(show) {
    const loadingElement = document.getElementById('loadingSpinner');
    if (loadingElement) {
        loadingElement.style.display = show ? 'block' : 'none';
    }
}

function hideMessages() {
    const errorElement = document.getElementById('errorMessage');
    const successElement = document.getElementById('successMessage');
    if (errorElement) errorElement.style.display = 'none';
    if (successElement) successElement.style.display = 'none';
}

function showError(message) {
    const errorElement = document.getElementById('errorMessage');
    if (errorElement) {
        errorElement.querySelector('.error-text').textContent = message;
        errorElement.style.display = 'block';
    }
}

function showSuccess(message) {
    const successElement = document.getElementById('successMessage');
    if (successElement) {
        successElement.querySelector('.success-text').textContent = message;
        successElement.style.display = 'block';
    }
}

async function createItem() {
    const form = document.getElementById('createItemForm');
    
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }

    const itemData = {
        itemName: document.getElementById('itemName').value.trim(),
        detail: document.getElementById('itemDetail').value.trim() || null,
        price: parseInt(document.getElementById('itemPrice').value) || 0,
        image: document.getElementById('itemImage').value.trim() || null,
        statusId: parseInt(document.getElementById('itemStatus').value) || 1
    };

    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');

        const response = await fetch('/api/manager/items', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify(itemData)
        });

        const result = await response.json();

        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('createItemModal'));
            modal.hide();
            showSuccess('Tạo item mới thành công!');
            loadItems(); // Refresh the list
        } else {
            showError(result.message || 'Lỗi khi tạo item mới');
        }
    } catch (error) {
        console.error('Error creating item:', error);
        showError('Lỗi khi tạo item mới: ' + error.message);
    }
}

async function viewItemDetails(itemId) {
    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');

        const response = await fetch(`/api/manager/items/${itemId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + authToken
            }
        });

        const result = await response.json();

        if (result.success) {
            populateViewModal(result.data);
            const modal = new bootstrap.Modal(document.getElementById('viewItemModal'));
            modal.show();
        } else {
            showError(result.message || 'Không thể tải thông tin item');
        }
    } catch (error) {
        console.error('Error fetching item details:', error);
        showError('Lỗi khi tải thông tin item');
    }
}

function populateViewModal(item) {
    document.getElementById('viewItemName').textContent = item.itemName;
    document.getElementById('viewItemDetail').textContent = item.detail || 'Không có mô tả';
    document.getElementById('viewItemPrice').textContent = formatCurrency(item.price);
    document.getElementById('viewItemStatus').textContent = item.statusName || 'Unknown';
    document.getElementById('viewItemStatus').className = `badge ${item.statusId === 1 ? 'bg-success' : 'bg-secondary'}`;
    document.getElementById('viewItemImage').src = item.image || '/images/default.jpg';
}

async function editItem(itemId) {
    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');

        const response = await fetch(`/api/manager/items/${itemId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + authToken
            }
        });

        const result = await response.json();

        if (result.success) {
            currentEditingItem = result.data;
            populateEditModal(result.data);
            const modal = new bootstrap.Modal(document.getElementById('editItemModal'));
            modal.show();
        } else {
            showError(result.message || 'Không thể tải thông tin item');
        }
    } catch (error) {
        console.error('Error fetching item details:', error);
        showError('Lỗi khi tải thông tin item');
    }
}

function populateEditModal(item) {
    document.getElementById('editItemId').value = item.itemId;
    document.getElementById('editItemName').value = item.itemName;
    document.getElementById('editItemDetail').value = item.detail || '';
    document.getElementById('editItemPrice').value = item.price;
    document.getElementById('editItemImage').value = item.image || '';
    document.getElementById('editItemStatus').value = item.statusId;
    updateEditItemPreview();
}

async function updateItem() {
    const form = document.getElementById('editItemForm');
    
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }

    const itemData = {
        itemId: parseInt(document.getElementById('editItemId').value),
        itemName: document.getElementById('editItemName').value.trim(),
        detail: document.getElementById('editItemDetail').value.trim() || null,
        price: parseInt(document.getElementById('editItemPrice').value) || 0,
        image: document.getElementById('editItemImage').value.trim() || null,
        statusId: parseInt(document.getElementById('editItemStatus').value)
    };

    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');

        const response = await fetch(`/api/manager/items/${itemData.itemId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + authToken
            },
            body: JSON.stringify(itemData)
        });

        const result = await response.json();

        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editItemModal'));
            modal.hide();
            showSuccess('Cập nhật item thành công!');
            loadItems(); // Refresh the list
        } else {
            showError(result.message || 'Lỗi khi cập nhật item');
        }
    } catch (error) {
        console.error('Error updating item:', error);
        showError('Lỗi khi cập nhật item: ' + error.message);
    }
}

async function deleteItem(itemId) {
    if (!confirm('Bạn có chắc chắn muốn xóa item này? Hành động này không thể hoàn tác.')) {
        return;
    }

    try {
        const authToken = localStorage.getItem('authToken') || getCookie('X-Access-Token');

        const response = await fetch(`/api/manager/items/${itemId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + authToken
            }
        });

        const result = await response.json();

        if (result.success) {
            showSuccess('Xóa item thành công!');
            loadItems(); // Refresh the list
        } else {
            showError(result.message || 'Lỗi khi xóa item');
        }
    } catch (error) {
        console.error('Error deleting item:', error);
        showError('Lỗi khi xóa item: ' + error.message);
    }
}

function updatePagination(pagination) {
    if (!pagination) return;
    
    totalPages = pagination.totalPages;
    currentPage = pagination.currentPage;
    
    const paginationContainer = document.getElementById('pagination');
    if (!paginationContainer) return;
    
    let paginationHtml = '';
    
    // Previous button
    paginationHtml += `
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage - 1})">Previous</a>
        </li>
    `;
    
    // Page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage || i === 1 || i === totalPages || (i >= currentPage - 1 && i <= currentPage + 1)) {
            paginationHtml += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                </li>
            `;
        } else if (i === currentPage - 2 || i === currentPage + 2) {
            paginationHtml += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }
    
    // Next button
    paginationHtml += `
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage + 1})">Next</a>
        </li>
    `;
    
    paginationContainer.innerHTML = paginationHtml;
}

function changePage(page) {
    if (page >= 1 && page <= totalPages && page !== currentPage) {
        currentPage = page;
        loadItems();
    }
}

// Utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}
