// Service Details JavaScript
let currentService = null;
let serviceStatuses = [];
let availableItems = [];
let itemsToRemove = [];
let newItemsCounter = 0;

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    console.log('Service Details page loaded');
    console.log('API_CONFIG:', API_CONFIG);
    console.log('Current URL:', window.location.href);
    
    const serviceId = getServiceIdFromUrl();
    console.log('Extracted Service ID:', serviceId);
    
    if (serviceId) {
        console.log('Starting to load service details, statuses, and available items...');
        loadServiceDetails(serviceId);
        loadServiceStatuses();
        loadAvailableItems();
    } else {
        showError('Service ID not found in URL');
        setTimeout(() => {
            goBack();
        }, 2000);
    }
});

// Get service ID from URL
function getServiceIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
}

// Load service details
async function loadServiceDetails(serviceId) {
    try {
        showLoading();
        const apiUrl = buildApiUrl(`${API_CONFIG.ENDPOINTS.SERVICES.DETAILS}/${serviceId}/details`);
        console.log('Loading service details from URL:', apiUrl);
        console.log('Service ID:', serviceId);
        
        const response = await fetch(apiUrl, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
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
            throw new Error(`Server responded with status ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Service details response:', result);
        console.log('Response data:', result.data);
        console.log('Response success:', result.success);
        console.log('Data type:', typeof result.data);
        
        if (result.success && result.data) {
            currentService = result.data;
            console.log('Current service set to:', currentService);
            displayServiceDetails(currentService);
        } else {
            console.error('Invalid response structure:', result);
            showError(result.message || 'Failed to load service details - Invalid response');
        }
    } catch (error) {
        console.error('Error loading service details:', error);
        
        if (error.message.includes('Failed to fetch') || error.message.includes('ERR_')) {
            showError('Không thể kết nối tới server. Vui lòng kiểm tra xem API server có đang chạy không.');
        } else {
            showError('Failed to load service details: ' + error.message);
        }
    } finally {
        hideLoading();
    }
}

// Display service details
function displayServiceDetails(service) {
    // Update header
    const serviceTitle = document.getElementById('serviceTitle');
    const serviceId = document.getElementById('serviceId');
    
    if (serviceTitle) serviceTitle.textContent = service.serviceName;
    if (serviceId) serviceId.textContent = service.serviceId;
    
    // Update service info
    const serviceName = document.getElementById('serviceName');
    const serviceStatus = document.getElementById('serviceStatus');
    const managerName = document.getElementById('managerName');
    const flightCount = document.getElementById('flightCount');
    const serviceDetail = document.getElementById('serviceDetail');
    
    if (serviceName) serviceName.textContent = service.serviceName;
    if (serviceStatus) {
        serviceStatus.textContent = service.status?.statusName || 'Unknown';
        serviceStatus.className = `badge ${getStatusBadgeClass(service.status?.statusName)}`;
    }
    if (managerName) managerName.textContent = service.managerName || 'Unknown';
    if (flightCount) flightCount.textContent = service.flightCount || 0;
    if (serviceDetail) serviceDetail.textContent = service.detail || 'No description available';
    
    // Update stats
    const itemCount = document.getElementById('itemCount');
    const flightCountStat = document.getElementById('flightCountStat');
    
    if (itemCount) itemCount.textContent = service.items?.length || 0;
    if (flightCountStat) flightCountStat.textContent = service.flightCount || 0;
    
    // Display items
    displayItems(service.items || []);
    
    // Add fade-in animation
    const container = document.querySelector('.service-details-container');
    if (container) container.classList.add('fade-in');
}

// Display items
function displayItems(items) {
    const itemsList = document.getElementById('itemsList');
    if (!itemsList) {
        console.error('itemsList element not found');
        return;
    }
    
    itemsList.innerHTML = '';
    
    if (items.length === 0) {
        itemsList.innerHTML = `
            <div class="col-12 text-center py-5">
                <div class="empty-state">
                    <i class="fas fa-box fa-3x text-muted mb-3"></i>
                    <h5 class="text-muted">No items found</h5>
                    <p class="text-muted">This service doesn't have any items yet.</p>
                    <button class="btn btn-primary" onclick="addNewItem()">
                        <i class="fas fa-plus"></i> Add First Item
                    </button>
                </div>
            </div>
        `;
        return;
    }
    
    items.forEach(item => {
        const itemCard = createItemCard(item);
        itemsList.appendChild(itemCard);
    });
}

// Create item card
function createItemCard(item) {
    const col = document.createElement('div');
    col.className = 'col-lg-4 col-md-6 mb-4';
    
    col.innerHTML = `
        <div class="item-card">
            <div class="item-header">
                <div class="w-100">
                    <h5 class="item-title">${item.itemName}</h5>
                    <div class="item-price">$${item.price.toLocaleString()}</div>
                </div>
                <span class="badge ${getStatusBadgeClass(item.statusName)}">${item.statusName || 'Unknown'}</span>
            </div>
            ${item.image ? `<img src="${item.image}" alt="${item.itemName}" class="item-image" onerror="this.style.display='none'">` : ''}
            <div class="item-description">
                ${item.detail || 'No description available'}
            </div>
            <div class="item-actions">
                <button class="btn btn-sm btn-outline-primary" onclick="editItem(${item.itemId})">
                    <i class="fas fa-edit"></i> Edit
                </button>
                <button class="btn btn-sm btn-outline-danger" onclick="removeItem(${item.itemId})">
                    <i class="fas fa-trash"></i> Remove
                </button>
            </div>
        </div>
    `;
    
    return col;
}

// Get status badge class
function getStatusBadgeClass(statusName) {
    if (!statusName) return 'bg-secondary';
    
    const status = statusName.toLowerCase();
    if (status.includes('active') || status.includes('available')) {
        return 'bg-success';
    } else if (status.includes('pending') || status.includes('review')) {
        return 'bg-warning';
    } else if (status.includes('inactive') || status.includes('disabled')) {
        return 'bg-danger';
    }
    return 'bg-secondary';
}

// Load service statuses
async function loadServiceStatuses() {
    try {
        console.log('Loading service statuses...');
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.SERVICES.STATUSES), {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            console.error('Failed to load service statuses:', response.status);
            return;
        }
        
        const result = await response.json();
        console.log('Service statuses response:', result);
        
        if (result.success) {
            serviceStatuses = result.data;
            console.log('Service statuses loaded:', serviceStatuses);
            console.log('Number of statuses:', serviceStatuses.length);
            serviceStatuses.forEach(status => {
                console.log(`Status: ID=${status.statusId}, Name=${status.statusName}, Type=${status.statusType}`);
            });
        } else {
            console.error('Failed to load service statuses:', result.message);
        }
    } catch (error) {
        console.error('Error loading service statuses:', error);
    }
}

// Load available items
async function loadAvailableItems() {
    try {
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.ITEMS.ACTIVE), {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            console.error('Failed to load available items:', response.status);
            return;
        }
        
        const result = await response.json();
        console.log('Available items response:', result);
        
        if (result.success) {
            availableItems = result.data;
            console.log('Available items loaded:', availableItems);
        } else {
            console.error('Failed to load available items:', result.message);
        }
    } catch (error) {
        console.error('Error loading available items:', error);
    }
}

// Populate status dropdown
function populateStatusDropdown() {
    const statusSelect = document.getElementById('editServiceStatus');
    if (!statusSelect) {
        console.error('Status select element not found');
        return;
    }
    
    console.log('Populating status dropdown with serviceStatuses:', serviceStatuses);
    statusSelect.innerHTML = '<option value="">Select Status</option>';
    
    if (serviceStatuses.length === 0) {
        console.warn('No service statuses available');
        // Add default options as fallback
        statusSelect.innerHTML += '<option value="1">Active</option>';
        statusSelect.innerHTML += '<option value="2">Inactive</option>';
        console.log('Added default status options');
        return;
    }
    
    serviceStatuses.forEach(status => {
        const option = document.createElement('option');
        option.value = status.statusId;
        option.textContent = status.statusName;
        statusSelect.appendChild(option);
        console.log(`Added status option: ${status.statusId} - ${status.statusName}`);
    });
    
    console.log('Status dropdown populated with', serviceStatuses.length, 'statuses');
}

// Open edit modal
function openEditModal() {
    if (!currentService) {
        showError('No service data available');
        return;
    }
    
    // Populate form
    document.getElementById('editServiceName').value = currentService.serviceName;
    document.getElementById('editServiceDetail').value = currentService.detail || '';
    
    // Reset items management
    itemsToRemove = [];
    newItemsCounter = 0;
    
    // Clear new items list
    document.getElementById('newItemsList').innerHTML = '';
    
    // Ensure statuses are loaded and populate dropdown
    if (serviceStatuses.length === 0) {
        // If statuses aren't loaded yet, load them first
        loadServiceStatuses().then(() => {
            populateStatusDropdown();
            document.getElementById('editServiceStatus').value = currentService.statusId || '';
        });
    } else {
        // Statuses are already loaded
        populateStatusDropdown();
        document.getElementById('editServiceStatus').value = currentService.statusId || '';
    }
    
    // Display existing items
    displayEditItems(currentService.items || []);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('editServiceModal'));
    modal.show();
}

// Display items in edit mode
function displayEditItems(items) {
    const editItemsList = document.getElementById('editItemsList');
    editItemsList.innerHTML = '';
    
    items.forEach(item => {
        const itemCard = createEditItemCard(item);
        editItemsList.appendChild(itemCard);
    });
}

// Create edit item card
function createEditItemCard(item) {
    const div = document.createElement('div');
    div.className = 'edit-item-card';
    div.setAttribute('data-item-id', item.itemId);
    
    div.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <div class="mb-3">
                    <label class="form-label">Item Name</label>
                    <input type="text" class="form-control" value="${item.itemName}" 
                           onchange="updateItemField(${item.itemId}, 'itemName', this.value)">
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-3">
                    <label class="form-label">Price</label>
                    <input type="number" class="form-control" value="${item.price}" 
                           onchange="updateItemField(${item.itemId}, 'price', this.value)">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-3">
                    <label class="form-label">Status</label>
                    <select class="form-select" onchange="updateItemField(${item.itemId}, 'statusId', this.value)">
                        <option value="">Select Status</option>
                        ${serviceStatuses.map(status => 
                            `<option value="${status.statusId}" ${status.statusId == item.statusId ? 'selected' : ''}>${status.statusName}</option>`
                        ).join('')}
                    </select>
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-3">
                    <label class="form-label">Image</label>
                    <input type="file" class="form-control" accept="image/*" 
                           onchange="previewEditItemImage(this, ${item.itemId})">
                    <div id="editImagePreview_${item.itemId}" class="mt-2">
                        ${item.image ? `<img src="${item.image}" alt="Current image" class="img-thumbnail" style="max-width: 100px; max-height: 100px;">` : ''}
                    </div>
                </div>
            </div>
        </div>
        <div class="mb-3">
            <label class="form-label">Description</label>
            <textarea class="form-control" rows="2" 
                      onchange="updateItemField(${item.itemId}, 'detail', this.value)">${item.detail || ''}</textarea>
        </div>
        <div class="item-form-actions">
            <button type="button" class="btn btn-outline-danger" onclick="markItemForRemoval(${item.itemId})">
                <i class="fas fa-trash"></i> Remove Item
            </button>
        </div>
    `;
    
    return div;
}

// Update item field
function updateItemField(itemId, field, value) {
    const item = currentService.items.find(i => i.itemId === itemId);
    if (item) {
        item[field] = field === 'price' ? parseInt(value) : value;
    }
}

// Mark item for removal
function markItemForRemoval(itemId) {
    if (!itemsToRemove.includes(itemId)) {
        itemsToRemove.push(itemId);
    }
    
    const itemCard = document.querySelector(`[data-item-id="${itemId}"]`);
    if (itemCard) {
        itemCard.classList.add('to-remove');
        itemCard.style.opacity = '0.5';
        
        // Change button to restore
        const removeBtn = itemCard.querySelector('.btn-outline-danger');
        removeBtn.innerHTML = '<i class="fas fa-undo"></i> Restore Item';
        removeBtn.onclick = () => restoreItem(itemId);
    }
}

// Restore item
function restoreItem(itemId) {
    const index = itemsToRemove.indexOf(itemId);
    if (index > -1) {
        itemsToRemove.splice(index, 1);
    }
    
    const itemCard = document.querySelector(`[data-item-id="${itemId}"]`);
    if (itemCard) {
        itemCard.classList.remove('to-remove');
        itemCard.style.opacity = '1';
        
        // Change button back to remove
        const removeBtn = itemCard.querySelector('.btn-outline-danger');
        removeBtn.innerHTML = '<i class="fas fa-trash"></i> Remove Item';
        removeBtn.onclick = () => markItemForRemoval(itemId);
    }
}

// Add new item to form
function addNewItemToForm() {
    const newItemsList = document.getElementById('newItemsList');
    const newItemCard = createNewItemCard();
    newItemsList.appendChild(newItemCard);
    newItemsCounter++;
}

// Create new item card
function createNewItemCard() {
    const div = document.createElement('div');
    div.className = 'new-item-card';
    div.setAttribute('data-new-item-id', newItemsCounter);
    
    div.innerHTML = `
        <h6><i class="fas fa-plus text-success"></i> New Item</h6>
        
        <!-- Option to create new item or select existing -->
        <div class="mb-3">
            <div class="form-check form-check-inline">
                <input class="form-check-input" type="radio" name="itemType_${newItemsCounter}" id="newItem_${newItemsCounter}" value="new" checked onchange="toggleItemType(${newItemsCounter}, 'new')">
                <label class="form-check-label" for="newItem_${newItemsCounter}">
                    Create New Item
                </label>
            </div>
            <div class="form-check form-check-inline">
                <input class="form-check-input" type="radio" name="itemType_${newItemsCounter}" id="existingItem_${newItemsCounter}" value="existing" onchange="toggleItemType(${newItemsCounter}, 'existing')">
                <label class="form-check-label" for="existingItem_${newItemsCounter}">
                    Select Existing Item
                </label>
            </div>
        </div>
        
        <!-- Existing items dropdown -->
        <div id="existingItemSection_${newItemsCounter}" class="mb-3" style="display: none;">
            <label class="form-label">Select Existing Item *</label>
            <select class="form-select" id="existingItemSelect_${newItemsCounter}">
                <option value="">Select an item...</option>
                ${availableItems.map(item => 
                    `<option value="${item.itemId}" data-name="${item.itemName}" data-price="${item.price}" data-detail="${item.detail || ''}" data-image="${item.image || ''}">${item.itemName} - $${item.price}</option>`
                ).join('')}
            </select>
        </div>
        
        <!-- New item form -->
        <div id="newItemSection_${newItemsCounter}">
            <div class="row">
                <div class="col-md-6">
                    <div class="mb-3">
                        <label class="form-label">Item Name *</label>
                        <input type="text" class="form-control" required>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="mb-3">
                        <label class="form-label">Price *</label>
                        <input type="number" class="form-control" required>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="mb-3">
                        <label class="form-label">Status</label>
                        <select class="form-select">
                            <option value="">Select Status</option>
                            ${serviceStatuses.map(status => 
                                `<option value="${status.statusId}">${status.statusName}</option>`
                            ).join('')}
                        </select>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="mb-3">
                        <label class="form-label">Image</label>
                        <input type="file" class="form-control" accept="image/*" onchange="previewImage(this, ${newItemsCounter})">
                        <div id="imagePreview_${newItemsCounter}" class="mt-2"></div>
                    </div>
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label">Description</label>
                <textarea class="form-control" rows="2"></textarea>
            </div>
        </div>
        
        <div class="item-form-actions">
            <button type="button" class="btn btn-outline-danger" onclick="removeNewItem(${newItemsCounter})">
                <i class="fas fa-trash"></i> Remove
            </button>
        </div>
    `;
    
    return div;
}

// Remove new item
function removeNewItem(newItemId) {
    const newItemCard = document.querySelector(`[data-new-item-id="${newItemId}"]`);
    if (newItemCard) {
        newItemCard.remove();
    }
}

// Save service
async function saveService() {
    try {
        const formData = await gatherFormData();
        
        if (!validateFormData(formData)) {
            return;
        }
        
        console.log('Saving service with data:', formData);
        showLoading();
        
        const response = await fetch(buildApiUrl(`${API_CONFIG.ENDPOINTS.SERVICES.UPDATE}/${currentService.serviceId}/advanced`), {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include',
            body: JSON.stringify(formData)
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                showError('Authentication required. Please login again.');
                setTimeout(() => {
                    window.location.href = '/Authentication/Login';
                }, 2000);
                return;
            }
            throw new Error(`Server responded with status ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Save service response:', result);
        
        if (result.success) {
            showSuccess('Service updated successfully');
            
            // Reload service details to get updated data
            await loadServiceDetails(currentService.serviceId);
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('editServiceModal'));
            if (modal) {
                modal.hide();
            }
        } else {
            showError(result.message || 'Failed to update service');
        }
    } catch (error) {
        console.error('Error saving service:', error);
        showError('Failed to save service: ' + error.message);
    } finally {
        hideLoading();
    }
}

// Gather form data
async function gatherFormData() {
    const formData = {
        serviceId: currentService.serviceId,
        serviceName: document.getElementById('editServiceName').value,
        detail: document.getElementById('editServiceDetail').value,
        statusId: document.getElementById('editServiceStatus').value || null,
        items: [],
        newItems: [],
        existingItemIds: [],
        itemIdsToRemove: itemsToRemove
    };
    
    // Gather existing items updates
    currentService.items.forEach(item => {
        if (!itemsToRemove.includes(item.itemId)) {
            formData.items.push({
                itemId: item.itemId,
                itemName: item.itemName,
                detail: item.detail,
                price: item.price,
                statusId: item.statusId,
                image: item.image
            });
        }
    });
    
    // Gather new items
    const newItemCards = document.querySelectorAll('.new-item-card');
    
    for (let card of newItemCards) {
        const itemId = card.getAttribute('data-new-item-id');
        const itemType = card.querySelector(`input[name="itemType_${itemId}"]:checked`).value;
        
        if (itemType === 'existing') {
            // Adding existing item
            const existingItemSelect = card.querySelector(`#existingItemSelect_${itemId}`);
            if (existingItemSelect.value) {
                formData.existingItemIds.push(parseInt(existingItemSelect.value));
            }
        } else {
            // Creating new item
            const inputs = card.querySelectorAll('#newItemSection_' + itemId + ' input, #newItemSection_' + itemId + ' select, #newItemSection_' + itemId + ' textarea');
            const fileInput = card.querySelector(`#newItemSection_${itemId} input[type="file"]`);
            
            let imageData = null;
            if (fileInput && fileInput.files && fileInput.files[0]) {
                try {
                    imageData = await fileToBase64(fileInput.files[0]);
                } catch (error) {
                    console.error('Error converting file to base64:', error);
                }
            }
            
            const itemData = {
                itemName: inputs[0].value,
                price: parseInt(inputs[1].value) || 0,
                statusId: inputs[2].value || null,
                image: imageData,
                detail: inputs[3].value || null
            };
            
            if (itemData.itemName && itemData.price > 0) {
                formData.newItems.push(itemData);
            }
        }
    }
    
    return formData;
}

// Validate form data
function validateFormData(formData) {
    if (!formData.serviceName.trim()) {
        showError('Service name is required');
        return false;
    }
    
    return true;
}

// Delete service
async function deleteService() {
    if (!currentService) {
        showError('No service data available');
        return;
    }
    
    const result = await showConfirm(
        'Delete Service',
        `Are you sure you want to delete "${currentService.serviceName}"? This action cannot be undone.`
    );
    
    if (result) {
        try {
            showLoading();
            
            const response = await fetch(buildApiUrl(`${API_CONFIG.ENDPOINTS.SERVICES.DELETE}/${currentService.serviceId}`), {
                method: 'DELETE',
                headers: {
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
                throw new Error(`Server responded with status ${response.status}`);
            }
            
            const result = await response.json();
            
            if (result.success) {
                showSuccess('Service deleted successfully');
                setTimeout(() => {
                    window.location.href = '/Manager/Services';
                }, 1500);
            } else {
                showError(result.message || 'Failed to delete service');
            }
        } catch (error) {
            console.error('Error deleting service:', error);
            showError('Failed to delete service: ' + error.message);
        } finally {
            hideLoading();
        }
    }
}

// Go back
function goBack() {
    window.history.back();
}

// Item management functions
function addNewItem() {
    // Open the edit modal and scroll to new items section
    if (!currentService) {
        showError('Please load service details first');
        return;
    }
    
    openEditModal();
    
    // Wait for modal to open, then add a new item
    setTimeout(() => {
        addNewItemToForm();
        
        // Scroll to new items section
        const newItemsList = document.getElementById('newItemsList');
        if (newItemsList) {
            newItemsList.scrollIntoView({ behavior: 'smooth' });
        }
    }, 500);
}

function editItem(itemId) {
    if (!currentService) {
        showError('Please load service details first');
        return;
    }
    
    // Open edit modal and highlight the specific item
    openEditModal();
    
    // Wait for modal to open, then highlight the item
    setTimeout(() => {
        const itemCard = document.querySelector(`[data-item-id="${itemId}"]`);
        if (itemCard) {
            itemCard.style.border = '2px solid #007bff';
            itemCard.scrollIntoView({ behavior: 'smooth' });
            
            // Remove highlight after 3 seconds
            setTimeout(() => {
                itemCard.style.border = '';
            }, 3000);
        }
    }, 500);
}

function removeItem(itemId) {
    if (!currentService) {
        showError('Please load service details first');
        return;
    }
    
    const item = currentService.items.find(i => i.itemId === itemId);
    if (!item) {
        showError('Item not found');
        return;
    }
    
    if (confirm(`Are you sure you want to remove "${item.itemName}" from this service?`)) {
        // Open edit modal and mark item for removal
        openEditModal();
        
        // Wait for modal to open, then mark item for removal
        setTimeout(() => {
            markItemForRemoval(itemId);
        }, 500);
    }
}

// Remove duplicate function definitions - using the improved openEditModal above

function deleteService() {
    if (!currentService) {
        showError('No service data available');
        return;
    }
    
    const result = confirm(`Are you sure you want to delete "${currentService.serviceName}"? This action cannot be undone.`);
    
    if (result) {
        // Call the async deleteService function
        deleteServiceAsync();
    }
}

// Async delete service function
async function deleteServiceAsync() {
    try {
        showLoading();
        
        const response = await fetch(buildApiUrl(`${API_CONFIG.ENDPOINTS.SERVICES.DELETE}/${currentService.serviceId}`), {
            method: 'DELETE',
            headers: {
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
            throw new Error(`Server responded with status ${response.status}`);
        }
        
        const result = await response.json();
        
        if (result.success) {
            showSuccess('Service deleted successfully');
            setTimeout(() => {
                window.location.href = '/Manager/Services';
            }, 1500);
        } else {
            showError(result.message || 'Failed to delete service');
        }
    } catch (error) {
        console.error('Error deleting service:', error);
        showError('Failed to delete service: ' + error.message);
    } finally {
        hideLoading();
    }
}

// Show confirmation dialog
function showConfirm(title, message) {
    return new Promise((resolve) => {
        const result = confirm(`${title}\n\n${message}`);
        resolve(result);
    });
}

// Loading and UI functions
function showLoading() {
    // Remove existing loading overlay if any
    const existingOverlay = document.querySelector('.loading-overlay');
    if (existingOverlay) {
        existingOverlay.remove();
    }
    
    // Create loading overlay
    const overlay = document.createElement('div');
    overlay.className = 'loading-overlay';
    overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(255, 255, 255, 0.9);
        z-index: 9999;
        display: flex;
        justify-content: center;
        align-items: center;
    `;
    
    overlay.innerHTML = `
        <div class="text-center">
            <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Loading...</span>
            </div>
            <div class="mt-3">
                <h5>Loading Service Details...</h5>
                <p class="text-muted">Please wait while we fetch the service information.</p>
            </div>
        </div>
    `;
    
    document.body.appendChild(overlay);
}

function hideLoading() {
    const overlay = document.querySelector('.loading-overlay');
    if (overlay) {
        overlay.remove();
    }
}

function showSuccess(message) {
    showToast(message, 'success');
}

function showError(message) {
    console.error('Error:', message);
    showToast(message, 'danger');
}

function showInfo(message) {
    showToast(message, 'info');
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast show align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${escapeHtml(message)}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '9999';
        document.body.appendChild(toastContainer);
    }
    
    toastContainer.appendChild(toast);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        toast.remove();
    }, 5000);
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

function buildApiUrl(endpoint) {
    // Check if API_CONFIG is available
    if (typeof API_CONFIG === 'undefined') {
        console.error('API_CONFIG is not defined');
        return `http://localhost:5000/api/Manager${endpoint}`;
    }
    
    return `${API_CONFIG.BASE_URL}${endpoint}`;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Navigation functions
function goBack() {
    window.history.back();
}

// Toggle item type (new or existing)
function toggleItemType(itemId, type) {
    const newSection = document.getElementById(`newItemSection_${itemId}`);
    const existingSection = document.getElementById(`existingItemSection_${itemId}`);
    
    if (type === 'new') {
        newSection.style.display = 'block';
        existingSection.style.display = 'none';
    } else {
        newSection.style.display = 'none';
        existingSection.style.display = 'block';
    }
}

// Preview image for new items
function previewImage(input, itemId) {
    const previewDiv = document.getElementById(`imagePreview_${itemId}`);
    previewDiv.innerHTML = '';
    
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            previewDiv.innerHTML = `<img src="${e.target.result}" alt="Preview" class="img-thumbnail" style="max-width: 100px; max-height: 100px;">`;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Preview image for edit items
function previewEditItemImage(input, itemId) {
    const previewDiv = document.getElementById(`editImagePreview_${itemId}`);
    
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            previewDiv.innerHTML = `<img src="${e.target.result}" alt="Preview" class="img-thumbnail" style="max-width: 100px; max-height: 100px;">`;
            
            // Update the item data with the new image
            const item = currentService.items.find(i => i.itemId === itemId);
            if (item) {
                item.image = e.target.result; // Store base64 for now
            }
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Convert file to base64
function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

// Global functions for onclick handlers
window.goBack = goBack;
window.openEditModal = openEditModal;
window.addNewItem = addNewItem;
window.editItem = editItem;
window.removeItem = removeItem;
window.saveService = saveService;
window.deleteService = deleteService;
window.addNewItemToForm = addNewItemToForm;
window.removeNewItem = removeNewItem;
window.markItemForRemoval = markItemForRemoval;
window.restoreItem = restoreItem;
window.updateItemField = updateItemField;
window.toggleItemType = toggleItemType;
window.previewImage = previewImage;
window.previewEditItemImage = previewEditItemImage;
