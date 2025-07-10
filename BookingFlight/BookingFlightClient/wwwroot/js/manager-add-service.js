// Add Service JavaScript
document.addEventListener('DOMContentLoaded', function() {
    initializeAddService();
});

function initializeAddService() {
    console.log('Initializing Add Service...');
    
    // Setup form validation
    setupFormValidation();
    
    // Setup preview updates
    setupPreviewUpdates();
    
    // Setup form submission
    setupFormSubmission();
    
    // Load available items
    loadAvailableItems();
    
    // Initialize preview with default values
    updatePreview();
}

async function loadServiceStatuses() {
    try {
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.SERVICES.STATUSES), {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                showError('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Service statuses response:', result);
        
        if (result.success) {
            populateStatusOptions(result.data || []);
        } else {
            console.error('Failed to load service statuses:', result.message);
            // Use default statuses if API fails
            populateStatusOptions([
                { statusId: 1, statusName: 'Hoạt động' },
                { statusId: 2, statusName: 'Không hoạt động' }
            ]);
        }
    } catch (error) {
        console.error('Error loading service statuses:', error);
        // Use default statuses if API fails
        populateStatusOptions([
            { statusId: 1, statusName: 'Hoạt động' },
            { statusId: 2, statusName: 'Không hoạt động' }
        ]);
    }
}

function populateStatusOptions(statuses) {
    const statusSelect = document.getElementById('serviceStatus');
    if (!statusSelect) return;
    
    // Clear existing options except the first one
    statusSelect.innerHTML = '<option value="">Chọn trạng thái...</option>';
    
    statuses.forEach(status => {
        const option = document.createElement('option');
        option.value = status.statusId;
        option.textContent = status.statusName;
        statusSelect.appendChild(option);
    });
    
    // Set default to "Hoạt động" if available
    const activeOption = statusSelect.querySelector('option[value="1"]');
    if (activeOption) {
        statusSelect.value = '1';
        updatePreview();
    }
}

function setupFormValidation() {
    const form = document.getElementById('addServiceForm');
    const serviceNameInput = document.getElementById('serviceName');
    
    if (serviceNameInput) {
        serviceNameInput.addEventListener('input', function() {
            validateServiceName(this);
        });
        
        serviceNameInput.addEventListener('blur', function() {
            validateServiceName(this);
        });
    }
    
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (validateForm()) {
                submitForm();
            }
        });
    }
}

function validateServiceName(input) {
    const value = input.value.trim();
    const isValid = value.length >= 3;
    
    if (isValid) {
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    } else {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');
    }
    
    return isValid;
}

function validateForm() {
    const form = document.getElementById('addServiceForm');
    const serviceNameInput = document.getElementById('serviceName');
    
    let isValid = true;
    
    // Validate service name
    if (!validateServiceName(serviceNameInput)) {
        isValid = false;
    }
    
    // Add Bootstrap validation classes
    form.classList.add('was-validated');
    
    return isValid;
}

function setupPreviewUpdates() {
    const serviceNameInput = document.getElementById('serviceName');
    const serviceDetailInput = document.getElementById('serviceDetail');
    
    if (serviceNameInput) {
        serviceNameInput.addEventListener('input', updatePreview);
    }
    
    if (serviceDetailInput) {
        serviceDetailInput.addEventListener('input', updatePreview);
    }
}

function updatePreview() {
    const serviceName = document.getElementById('serviceName')?.value || 'Tên Dịch Vụ';
    const serviceDetail = document.getElementById('serviceDetail')?.value || 'Mô tả sẽ hiển thị ở đây...';
    
    // Update preview title
    const previewTitle = document.getElementById('previewTitle');
    if (previewTitle) {
        previewTitle.textContent = serviceName;
    }
    
    // Update preview description
    const previewDescription = document.getElementById('previewDescription');
    if (previewDescription) {
        previewDescription.textContent = serviceDetail;
    }
    
    // Status is always "Hoạt động" (Active)
    const previewStatus = document.getElementById('previewStatus');
    if (previewStatus) {
        previewStatus.textContent = 'Hoạt động';
        previewStatus.className = 'badge bg-success';
    }
}

function getStatusClass(statusId) {
    switch (statusId) {
        case '1': return 'bg-success';
        case '2': return 'bg-secondary';
        default: return 'bg-secondary';
    }
}

function setupFormSubmission() {
    const form = document.getElementById('addServiceForm');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (validateForm()) {
                submitForm();
            }
        });
    }
}

async function submitForm() {
    // Prevent multiple submissions
    if (window.isSubmitting) {
        console.log('Form is already being submitted, ignoring duplicate request');
        return;
    }
    
    window.isSubmitting = true;
    
    try {
        showLoadingOverlay();
        
        const formData = getFormData();
        console.log('Submitting form data:', formData);
        
        const token = getAuthToken();
        console.log('Using auth token:', token ? 'Token present' : 'No token found');
        
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.SERVICES.CREATE), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include',
            body: JSON.stringify(formData)
        });
        
        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers);
        
        if (!response.ok) {
            if (response.status === 401) {
                showError('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            const errorText = await response.text();
            console.error('Error response:', errorText);
            throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
        }
        
        const result = await response.json();
        console.log('Create service response:', result);
        
        if (result.success) {
            showSuccessMessage();
            setTimeout(() => {
                window.location.href = '/Manager/Services';
            }, 2000);
        } else {
            throw new Error(result.message || 'Failed to create service');
        }
        
    } catch (error) {
        console.error('Error creating service:', error);
        showErrorMessage(error.message);
    } finally {
        hideLoadingOverlay();
        window.isSubmitting = false;
    }
}

function getFormData() {
    const serviceName = document.getElementById('serviceName').value.trim();
    const serviceDetail = document.getElementById('serviceDetail').value.trim();
    
    return {
        serviceName: serviceName,
        detail: serviceDetail || null,
        managerId: getCurrentManagerId(),
        statusId: 1, // Always set to Active (1) by default
        itemIds: selectedItemIds // Include selected item IDs
    };
}

function getCurrentManagerId() {
    // Try to get manager ID from JWT token or default to 1
    try {
        const token = getAuthToken();
        if (token) {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return parseInt(payload.ManagerId) || 1;
        }
    } catch (error) {
        console.error('Error getting manager ID from token:', error);
    }
    return 1; // Default manager ID
}

function showLoadingOverlay() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.style.display = 'flex';
    }
}

function hideLoadingOverlay() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.style.display = 'none';
    }
}

function showSuccessMessage() {
    const overlay = document.getElementById('loadingOverlay');
    const loadingContent = overlay.querySelector('.loading-content');
    
    if (loadingContent) {
        loadingContent.innerHTML = `
            <div class="text-success">
                <i class="fas fa-check-circle fa-3x mb-3"></i>
                <h5>Thành công!</h5>
                <p>Dịch vụ đã được tạo thành công.</p>
                <p class="small text-muted">Đang chuyển hướng...</p>
            </div>
        `;
    }
}

function showErrorMessage(message) {
    const overlay = document.getElementById('loadingOverlay');
    const loadingContent = overlay.querySelector('.loading-content');
    
    if (loadingContent) {
        loadingContent.innerHTML = `
            <div class="text-danger">
                <i class="fas fa-exclamation-triangle fa-3x mb-3"></i>
                <h5>Lỗi!</h5>
                <p>${escapeHtml(message)}</p>
                <button class="btn btn-primary mt-3" onclick="hideLoadingOverlay()">
                    <i class="fas fa-times me-2"></i>Đóng
                </button>
            </div>
        `;
    }
}

function resetForm() {
    const form = document.getElementById('addServiceForm');
    if (form) {
        form.reset();
        form.classList.remove('was-validated');
        
        // Reset validation classes
        const inputs = form.querySelectorAll('.form-control, .form-select');
        inputs.forEach(input => {
            input.classList.remove('is-valid', 'is-invalid');
        });
        
        // Reset status to default (Active)
        const statusInput = document.getElementById('serviceStatus');
        if (statusInput) {
            statusInput.value = '1';
        }
        
        // Reset selected items
        selectedItemIds = [];
        const checkboxes = document.querySelectorAll('.item-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.checked = false;
            const container = checkbox.closest('.item-checkbox-container');
            if (container) {
                container.classList.remove('selected');
            }
        });
        
        // Reset preview
        updatePreview();
    }
}

// Items management
let selectedItemIds = [];

async function loadAvailableItems() {
    console.log('Loading available items...');
    
    const itemsLoading = document.getElementById('itemsLoading');
    const itemsList = document.getElementById('itemsList');
    const noItemsMessage = document.getElementById('noItemsMessage');
    
    try {
        // Show loading
        if (itemsLoading) itemsLoading.style.display = 'block';
        if (itemsList) itemsList.style.display = 'none';
        if (noItemsMessage) noItemsMessage.style.display = 'none';
        
        // Use the GET endpoint for active items
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.ITEMS.ACTIVE), {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                showError('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Items response:', result);
        
        if (result.success && result.data && result.data.length > 0) {
            displayItems(result.data);
        } else {
            showNoItemsMessage();
        }
    } catch (error) {
        console.error('Error loading items:', error);
        showNoItemsMessage();
    } finally {
        // Hide loading
        if (itemsLoading) itemsLoading.style.display = 'none';
    }
}

function displayItems(items) {
    const itemsList = document.getElementById('itemsList');
    if (!itemsList) return;
    
    itemsList.innerHTML = items.map(item => createItemCheckbox(item)).join('');
    itemsList.style.display = 'block';
    
    // Add event listeners for checkboxes
    items.forEach(item => {
        const checkbox = document.getElementById(`item_${item.itemId}`);
        const container = document.querySelector(`[data-item-id="${item.itemId}"]`);
        
        if (checkbox && container) {
            checkbox.addEventListener('change', function() {
                handleItemSelection(item, this.checked);
                updateItemContainerStyle(container, this.checked);
            });
            
            container.addEventListener('click', function(e) {
                if (e.target !== checkbox) {
                    checkbox.click();
                }
            });
        }
    });
}

function createItemCheckbox(item) {
    return `
        <div class="item-checkbox-container" data-item-id="${item.itemId}">
            <input type="checkbox" class="form-check-input item-checkbox" id="item_${item.itemId}" value="${item.itemId}">
            <div class="item-info">
                <div class="item-name">${escapeHtml(item.itemName)}</div>
                <div class="item-detail">${escapeHtml(item.detail || '')}</div>
                <div class="item-price">${formatPrice(item.price)} VND</div>
            </div>
        </div>
    `;
}

function handleItemSelection(item, isSelected) {
    if (isSelected) {
        if (!selectedItemIds.includes(item.itemId)) {
            selectedItemIds.push(item.itemId);
        }
    } else {
        selectedItemIds = selectedItemIds.filter(id => id !== item.itemId);
    }
    
    console.log('Selected item IDs:', selectedItemIds);
    updateSelectedItemsPreview();
    updatePreview();
}

function updateItemContainerStyle(container, isSelected) {
    if (isSelected) {
        container.classList.add('selected');
    } else {
        container.classList.remove('selected');
    }
}

function updateSelectedItemsPreview() {
    const previewContainer = document.getElementById('selectedItemsPreview');
    const countSpan = document.getElementById('selectedCount');
    const tagsContainer = document.getElementById('selectedItemsTags');
    
    if (!previewContainer || !countSpan || !tagsContainer) return;
    
    countSpan.textContent = selectedItemIds.length;
    
    if (selectedItemIds.length > 0) {
        previewContainer.style.display = 'block';
        
        // Get selected items info
        const selectedItemsInfo = [];
        selectedItemIds.forEach(itemId => {
            const checkbox = document.getElementById(`item_${itemId}`);
            if (checkbox) {
                const container = checkbox.closest('.item-checkbox-container');
                const nameElement = container?.querySelector('.item-name');
                if (nameElement) {
                    selectedItemsInfo.push({
                        id: itemId,
                        name: nameElement.textContent
                    });
                }
            }
        });
        
        // Create tags
        tagsContainer.innerHTML = selectedItemsInfo.map(item => `
            <span class="selected-item-tag">
                ${escapeHtml(item.name)}
                <span class="remove-item" onclick="removeSelectedItem(${item.id})">
                    <i class="fas fa-times"></i>
                </span>
            </span>
        `).join('');
    } else {
        previewContainer.style.display = 'none';
    }
}

function removeSelectedItem(itemId) {
    // Uncheck the checkbox
    const checkbox = document.getElementById(`item_${itemId}`);
    if (checkbox && checkbox.checked) {
        checkbox.click(); // This will trigger the change event and update selections
    }
}

function showNoItemsMessage() {
    const itemsList = document.getElementById('itemsList');
    const noItemsMessage = document.getElementById('noItemsMessage');
    
    if (itemsList) itemsList.style.display = 'none';
    if (noItemsMessage) noItemsMessage.style.display = 'block';
}

function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN').format(price);
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

function showError(message) {
    console.error('Error:', message);
    alert(message); // Simple error display - you can enhance this
}

// Modal functions
function showAddItemModal() {
    const modal = new bootstrap.Modal(document.getElementById('addItemModal'));
    modal.show();
    
    // Reset form
    document.getElementById('addItemForm').reset();
    
    // Reset image preview
    const imagePreview = document.getElementById('imagePreview');
    const previewItemImage = document.getElementById('previewItemImage');
    if (imagePreview) {
        imagePreview.style.display = 'none';
    }        if (previewItemImage) {
            previewItemImage.src = '/images/default.jpg';
        }
    
    // Setup modal preview updates
    setupModalPreviewUpdates();
    
    // Clear any previous validation
    const form = document.getElementById('addItemForm');
    form.classList.remove('was-validated');
    
    // Update preview with empty values
    updateModalPreview();
}

function setupModalPreviewUpdates() {
    const itemNameInput = document.getElementById('itemName');
    const itemDetailInput = document.getElementById('itemDetail');
    const itemPriceInput = document.getElementById('itemPrice');
    const itemImageInput = document.getElementById('itemImage');
    
    if (itemNameInput) {
        itemNameInput.addEventListener('input', updateModalPreview);
    }
    
    if (itemDetailInput) {
        itemDetailInput.addEventListener('input', updateModalPreview);
    }
    
    if (itemPriceInput) {
        itemPriceInput.addEventListener('input', updateModalPreview);
    }
    
    if (itemImageInput) {
        itemImageInput.addEventListener('change', handleImagePreview);
    }
}

function handleImagePreview(event) {
    const file = event.target.files[0];
    const imagePreview = document.getElementById('imagePreview');
    const previewImg = document.getElementById('previewImg');
    const previewItemImage = document.getElementById('previewItemImage');
    
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            const imageSrc = e.target.result;
            
            // Update preview in modal
            if (previewImg) {
                previewImg.src = imageSrc;
            }
            if (imagePreview) {
                imagePreview.style.display = 'block';
            }
            
            // Update preview in preview card
            if (previewItemImage) {
                previewItemImage.src = imageSrc;
            }
        };
        reader.readAsDataURL(file);
    } else {
        // Reset to default image
        if (imagePreview) {
            imagePreview.style.display = 'none';
        }
        if (previewItemImage) {
            previewItemImage.src = '/images/default.jpg';
        }
    }
}

function updateModalPreview() {
    const itemName = document.getElementById('itemName')?.value || 'Tên Item';
    const itemDetail = document.getElementById('itemDetail')?.value || 'Mô tả chi tiết...';
    const itemPrice = document.getElementById('itemPrice')?.value || 0;
    
    const previewName = document.getElementById('previewItemName');
    const previewDetail = document.getElementById('previewItemDetail');
    const previewPrice = document.getElementById('previewItemPrice');
    
    if (previewName) previewName.textContent = itemName;
    if (previewDetail) previewDetail.textContent = itemDetail;
    if (previewPrice) previewPrice.textContent = formatPrice(itemPrice) + ' VND';
}

async function submitAddItem() {
    const form = document.getElementById('addItemForm');
    
    // Validate form
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }
    
    // Prepare form data for file upload
    const formData = new FormData(form);
    const fileInput = document.getElementById('itemImage');
    let imageFileName = null;
    
    // Get filename if selected
    if (fileInput && fileInput.files && fileInput.files[0]) {
        imageFileName = fileInput.files[0].name;
        console.log('Image file selected:', imageFileName);
    } else {
        console.log('No image file selected');
    }
    
    // Prepare data
    const itemData = {
        itemName: formData.get('itemName'),
        detail: formData.get('itemDetail'),
        price: parseInt(formData.get('itemPrice')) || 0,
        statusId: 1, // Active by default
        image: imageFileName || 'default.jpg' // Use filename instead of base64
    };
    
    console.log('Submitting item data:', itemData);
    
    try {
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.ITEMS.CREATE), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include',
            body: JSON.stringify(itemData)
        });
        
        console.log('Response status:', response.status);
        
        if (!response.ok) {
            if (response.status === 401) {
                showError('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            const errorText = await response.text();
            console.error('Error response:', errorText);
            throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
        }
        
        const result = await response.json();
        
        if (result.success) {
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('addItemModal'));
            modal.hide();
            
            // Show success message
            showNotification('Item được tạo thành công!', 'success');
            
            // Reload items list
            await loadAvailableItems();
            
            // Auto-select the new item
            setTimeout(() => {
                selectItemById(result.data.itemId);
            }, 100);
            
        } else {
            showNotification(result.message || 'Có lỗi xảy ra khi tạo item', 'error');
        }
    } catch (error) {
        console.error('Error creating item:', error);
        showNotification('Có lỗi xảy ra khi tạo item', 'error');
    }
}

function convertFileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = function(e) {
            resolve(e.target.result);
        };
        reader.onerror = function(error) {
            reject(error);
        };
        reader.readAsDataURL(file);
    });
}

function showNotification(message, type = 'success') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    `;
    
    notification.innerHTML = `
        <div class="d-flex align-items-center">
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'} me-2"></i>
            <span>${message}</span>
        </div>
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 5000);
}

function selectItemById(itemId) {
    const checkbox = document.getElementById(`item_${itemId}`);
    if (checkbox && !checkbox.checked) {
        checkbox.click(); // This will trigger the change event and update selections
    }
}

// Global functions
window.resetForm = resetForm;
window.showAddItemModal = showAddItemModal;
window.submitAddItem = submitAddItem;
