// Add Item Modal functions
function showAddItemModal() {
    const modal = new bootstrap.Modal(document.getElementById('addItemModal'));
    modal.show();
    
    // Setup modal preview updates
    setupAddItemPreview();
    
    // Reset form
    resetAddItemForm();
}

function setupAddItemPreview() {
    const itemNameInput = document.getElementById('itemName');
    const itemDetailInput = document.getElementById('itemDetail');
    const itemPriceInput = document.getElementById('itemPrice');
    
    if (itemNameInput) {
        itemNameInput.addEventListener('input', updateAddItemPreview);
    }
    if (itemDetailInput) {
        itemDetailInput.addEventListener('input', updateAddItemPreview);
    }
    if (itemPriceInput) {
        itemPriceInput.addEventListener('input', updateAddItemPreview);
    }
    
    // Initial preview update
    updateAddItemPreview();
}

function updateAddItemPreview() {
    const itemName = document.getElementById('itemName')?.value || 'Tên Item';
    const itemDetail = document.getElementById('itemDetail')?.value || 'Mô tả chi tiết...';
    const itemPrice = document.getElementById('itemPrice')?.value || 0;
    
    // Update preview
    const previewName = document.getElementById('previewItemName');
    const previewDetail = document.getElementById('previewItemDetail');
    const previewPrice = document.getElementById('previewItemPrice');
    
    if (previewName) previewName.textContent = itemName;
    if (previewDetail) previewDetail.textContent = itemDetail;
    if (previewPrice) previewPrice.textContent = formatPrice(itemPrice) + ' VND';
}

function resetAddItemForm() {
    const form = document.getElementById('addItemForm');
    if (form) {
        form.reset();
        form.classList.remove('was-validated');
        
        // Reset validation classes
        const inputs = form.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.classList.remove('is-valid', 'is-invalid');
        });
    }
    
    updateAddItemPreview();
}

async function submitAddItem() {
    const form = document.getElementById('addItemForm');
    if (!form) return;
    
    // Validate form
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }
    
    try {
        const itemData = getAddItemFormData();
        console.log('Creating item:', itemData);
        
        const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.ITEMS.CREATE), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            body: JSON.stringify(itemData)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Create item response:', result);
        
        if (result.success) {
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('addItemModal'));
            modal.hide();
            
            // Reload items list
            await loadAvailableItems();
            
            // Auto-select the new item
            if (result.data && result.data.itemId) {
                setTimeout(() => {
                    const checkbox = document.getElementById(`item_${result.data.itemId}`);
                    if (checkbox) {
                        checkbox.click();
                    }
                }, 500);
            }
            
            // Show success message
            showNotification('Item đã được tạo thành công!', 'success');
        } else {
            throw new Error(result.message || 'Failed to create item');
        }
        
    } catch (error) {
        console.error('Error creating item:', error);
        showNotification('Lỗi khi tạo item: ' + error.message, 'error');
    }
}

function getAddItemFormData() {
    const itemName = document.getElementById('itemName').value.trim();
    const itemDetail = document.getElementById('itemDetail').value.trim();
    const itemPrice = parseInt(document.getElementById('itemPrice').value) || 0;
    
    return {
        itemName: itemName,
        detail: itemDetail || null,
        price: itemPrice,
        statusId: 1 // Active by default
    };
}

function showNotification(message, type = 'success') {
    // Create a simple notification
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'success' ? 'success' : 'danger'} notification-toast`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        opacity: 0;
        transition: opacity 0.3s ease;
    `;
    notification.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
        ${message}
    `;
    
    document.body.appendChild(notification);
    
    // Fade in
    setTimeout(() => {
        notification.style.opacity = '1';
    }, 100);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        notification.style.opacity = '0';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    }, 3000);
}