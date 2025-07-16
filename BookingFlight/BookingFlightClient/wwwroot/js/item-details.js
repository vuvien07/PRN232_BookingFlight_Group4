// Item Details JavaScript

let currentItem = null;

document.addEventListener('DOMContentLoaded', function() {
    loadItemDetails();
    setupEventListeners();
    setupEditImageOptions();
});

function setupEventListeners() {
    // Image URL preview for edit modal
    document.getElementById('editImage').addEventListener('input', function() {
        previewEditImage(this.value);
    });
}

// Load item details function with modern styling
function loadItemDetails() {
    const urlParams = new URLSearchParams(window.location.search);
    const itemId = urlParams.get('id');

    if (!itemId) {
        showError('Item ID not found');
        return;
    }

    // Show loading
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('itemDetailsCard').style.display = 'none';

    fetch(`/api/items/${itemId}`)
        .then(response => {
            if (!response.ok) throw new Error('Item not found');
            return response.json();
        })
        .then(item => {
            currentItem = item;
            displayItemDetails(item);
            
            // Hide loading and show content with animation
            document.getElementById('loadingSpinner').style.display = 'none';
            const detailsCard = document.getElementById('itemDetailsCard');
            detailsCard.style.display = 'block';
            detailsCard.classList.add('fade-in');
        })
        .catch(error => {
            console.error('Error loading item:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            showError('Failed to load item details');
        });
}

function displayItemDetails(item) {
    // Update header title
    document.getElementById('headerItemName').textContent = item.itemName;
    
    // Update item details
    document.getElementById('itemName').textContent = item.itemName;
    document.getElementById('itemId').textContent = item.itemId;
    document.getElementById('itemPrice').textContent = formatPrice(item.price);
    document.getElementById('itemDetail').textContent = item.detail || 'No description available';
    
    // Update status with modern badge
    const statusElement = document.getElementById('itemStatus');
    const statusBadge = getStatusBadge(item.status);
    statusElement.innerHTML = statusBadge;
    
    // Update image with error handling
    const imageElement = document.getElementById('itemImage');
    const imagePath = resolveImagePath(item.image);
    imageElement.src = imagePath;
    imageElement.alt = item.itemName;
    
    // Handle image load error
    imageElement.onerror = function() {
        this.src = '/images/default-item.png';
        this.alt = 'Default item image';
    };
}

// Helper function to get status badge with modern styling
function getStatusBadge(status) {
    const statusMap = {
        1: { text: 'Active', class: 'status-active' },
        0: { text: 'Inactive', class: 'status-inactive' }
    };
    
    const statusInfo = statusMap[status] || { text: 'Unknown', class: 'status-inactive' };
    return `<span class="status-badge ${statusInfo.class}">${statusInfo.text}</span>`;
}

// Helper function to format price
function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
}

// Helper function to resolve image path
function resolveImagePath(imagePath) {
    if (!imagePath) return '/images/default-item.png';
    
    // If it's already a full URL, return as is
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
        return imagePath;
    }
    
    // If it already starts with /images/, return as is
    if (imagePath.startsWith('/images/')) {
        return imagePath;
    }
    
    // If it starts with images/ (without leading slash), add the slash
    if (imagePath.startsWith('images/')) {
        return '/' + imagePath;
    }
    
    // Otherwise, assume it's just a filename and prepend /images/
    return '/images/' + imagePath;
}

// Modern error display function
function showError(message) {
    const errorDiv = document.getElementById('errorMessage');
    const errorText = document.getElementById('errorText');
    
    errorText.textContent = message;
    errorDiv.style.display = 'block';
    errorDiv.classList.add('fade-in');
    
    // Auto-hide after 5 seconds
    setTimeout(() => {
        errorDiv.style.display = 'none';
        errorDiv.classList.remove('fade-in');
    }, 5000);
}

function setupEditImageOptions() {
    // Handle edit image option switching
    const editImageUrlRadio = document.getElementById('editImageUrl');
    const editImageUploadRadio = document.getElementById('editImageUpload');
    const editUrlContainer = document.getElementById('editUrlInputContainer');
    const editFileContainer = document.getElementById('editFileUploadContainer');
    const editImageFileInput = document.getElementById('editImageFile');
    const editUploadActions = document.getElementById('editUploadActions');
    const editUploadBtn = document.getElementById('editUploadBtn');
    const editCancelUploadBtn = document.getElementById('editCancelUploadBtn');

    editImageUrlRadio.addEventListener('change', function() {
        if (this.checked) {
            editUrlContainer.style.display = 'block';
            editFileContainer.style.display = 'none';
            resetEditFileUpload();
            hideEditImagePreview();
        }
    });

    editImageUploadRadio.addEventListener('change', function() {
        if (this.checked) {
            editUrlContainer.style.display = 'none';
            editFileContainer.style.display = 'block';
            document.getElementById('editImage').value = '';
            hideEditImagePreview();
        }
    });

    // Handle file selection
    editImageFileInput.addEventListener('change', function() {
        handleEditFileSelection(this.files);
    });

    // Handle drag and drop
    const editFileUploadBtn = document.getElementById('editFileUploadBtn');
    
    editFileUploadBtn.addEventListener('dragover', function(e) {
        e.preventDefault();
        this.classList.add('dragover');
    });

    editFileUploadBtn.addEventListener('dragleave', function(e) {
        e.preventDefault();
        this.classList.remove('dragover');
    });

    editFileUploadBtn.addEventListener('drop', function(e) {
        e.preventDefault();
        this.classList.remove('dragover');
        const files = e.dataTransfer.files;
        if (files.length > 0) {
            editImageFileInput.files = files;
            handleEditFileSelection(files);
        }
    });

    // Upload button click
    editUploadBtn.addEventListener('click', function() {
        uploadEditImage();
    });

    // Cancel upload button
    editCancelUploadBtn.addEventListener('click', function() {
        resetEditFileUpload();
    });
}

function handleEditFileSelection(files) {
    const editUploadActions = document.getElementById('editUploadActions');
    const editFileUploadBtn = document.getElementById('editFileUploadBtn');
    
    if (files && files[0]) {
        // Validate file
        const file = files[0];
        const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
        const maxSize = 5 * 1024 * 1024; // 5MB

        if (!validTypes.includes(file.type)) {
            showAlert('error', 'Please select a valid image file (JPEG, PNG, GIF)');
            resetEditFileUpload();
            return;
        }

        if (file.size > maxSize) {
            showAlert('error', 'File size must be less than 5MB');
            resetEditFileUpload();
            return;
        }

        // Update UI
        editFileUploadBtn.classList.add('file-selected');
        editFileUploadBtn.innerHTML = `
            <i class="fas fa-check-circle"></i>
            <div>File selected: ${file.name}</div>
            <div class="upload-info">Ready to upload (${formatFileSize(file.size)})</div>
        `;
        
        editUploadActions.style.display = 'block';
        
        // Preview the selected file
        const reader = new FileReader();
        reader.onload = function(e) {
            previewEditImage(e.target.result);
        };
        reader.readAsDataURL(file);
    } else {
        resetEditFileUpload();
    }
}

function resetEditFileUpload() {
    const editImageFileInput = document.getElementById('editImageFile');
    const editUploadActions = document.getElementById('editUploadActions');
    const editFileUploadBtn = document.getElementById('editFileUploadBtn');
    const editUploadProgress = document.getElementById('editUploadProgress');
    
    editImageFileInput.value = '';
    editUploadActions.style.display = 'none';
    editUploadProgress.style.display = 'none';
    
    editFileUploadBtn.classList.remove('file-selected');
    editFileUploadBtn.innerHTML = `
        <i class="fas fa-cloud-upload-alt"></i>
        <div>Click to select image or drag & drop</div>
        <div class="upload-info">JPEG, PNG, GIF files up to 5MB</div>
    `;
    
    hideEditImagePreview();
}

function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

async function uploadEditImage() {
    const fileInput = document.getElementById('editImageFile');
    const uploadBtn = document.getElementById('editUploadBtn');
    const uploadProgress = document.getElementById('editUploadProgress');
    const progressBar = uploadProgress.querySelector('.progress-bar');
    
    if (!fileInput.files || !fileInput.files[0]) {
        showAlert('warning', 'Please select a file to upload.');
        return;
    }

    const formData = new FormData();
    formData.append('imageFile', fileInput.files[0]);

    try {
        // Show progress
        uploadProgress.style.display = 'block';
        uploadBtn.disabled = true;
        uploadBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Uploading...';
        
        // Simulate progress
        let progress = 0;
        const progressInterval = setInterval(() => {
            progress += Math.random() * 20;
            if (progress > 90) progress = 90;
            progressBar.style.width = progress + '%';
        }, 100);

        const token = getAuthToken();
        const response = await fetch('/ItemManage/UploadImage', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });

        clearInterval(progressInterval);
        progressBar.style.width = '100%';

        const result = await response.json();

        if (result.success) {
            // Set the uploaded image URL to the hidden image input
            document.getElementById('editImage').value = result.imageUrl;
            showAlert('success', 'Image uploaded successfully!');
            
            // Update preview with the uploaded image
            previewEditImage(result.imageUrl);
            
            // Hide upload actions
            document.getElementById('editUploadActions').style.display = 'none';
            
            // Update file upload button to show success
            const editFileUploadBtn = document.getElementById('editFileUploadBtn');
            editFileUploadBtn.innerHTML = `
                <i class="fas fa-check-circle text-success"></i>
                <div class="text-success">Image uploaded successfully!</div>
                <div class="upload-info">${fileInput.files[0].name}</div>
            `;
        } else {
            showAlert('error', result.message || 'Upload failed');
            resetEditFileUpload();
        }
    } catch (error) {
        console.error('Upload error:', error);
        showAlert('error', 'Upload failed: ' + error.message);
        resetEditFileUpload();
    } finally {
        uploadBtn.disabled = false;
        uploadBtn.innerHTML = '<i class="fas fa-upload"></i> Upload Image';
        
        // Hide progress after a delay
        setTimeout(() => {
            uploadProgress.style.display = 'none';
            progressBar.style.width = '0%';
        }, 1000);
    }
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

async function loadItemDetails() {
    try {
        showLoading();

        const response = await fetch(`/ItemManage/GetItem?id=${itemId}`, {
            headers: getAuthHeaders()
        });

        const result = await response.json();

        if (result.success && result.data) {
            currentItem = result.data;
            displayItemDetails(result.data);
        } else {
            showError(result.message || 'Item not found');
        }
    } catch (error) {
        console.error('Error loading item details:', error);
        showError('Error loading item details: ' + error.message);
    } finally {
        hideLoading();
    }
}

function displayItemDetails(item) {
    // Update page elements
    document.getElementById('itemId').textContent = item.itemId;
    document.getElementById('itemName').textContent = item.itemName;
    document.getElementById('itemDetail').textContent = item.detail || 'No description available';
    document.getElementById('itemPrice').textContent = formatPrice(item.price);
    
    // Update image
    const itemImage = document.getElementById('itemImage');
    itemImage.src = getImagePath(item.image);
    itemImage.alt = item.itemName;
    itemImage.onerror = function() {
        handleImageError(this);
    };
    
    // Update status badge
    const statusBadge = document.getElementById('itemStatus');
    statusBadge.textContent = getStatusText(item.statusId);
    statusBadge.className = `badge ${getStatusClass(item.statusId)}`;
    
    // Show the card
    document.getElementById('itemDetailsCard').style.display = 'block';
    document.getElementById('itemDetailsCard').classList.add('fade-in');
}

function openEditModal() {
    if (!currentItem) {
        showAlert('error', 'Item data not loaded');
        return;
    }

    // Populate edit form
    document.getElementById('editItemId').value = currentItem.itemId;
    document.getElementById('editItemName').value = currentItem.itemName;
    document.getElementById('editDetail').value = currentItem.detail || '';
    document.getElementById('editPrice').value = currentItem.price;
    document.getElementById('editImage').value = currentItem.image || '';
    document.getElementById('editStatusId').value = currentItem.statusId || 1;
    
    // Preview image if exists
    if (currentItem.image) {
        previewEditImage(currentItem.image);
    } else {
        hideEditImagePreview();
    }

    const modal = new bootstrap.Modal(document.getElementById('editItemModal'));
    modal.show();
}

async function saveItemChanges() {
    try {
        const itemData = {
            itemId: parseInt(document.getElementById('editItemId').value),
            itemName: document.getElementById('editItemName').value.trim(),
            detail: document.getElementById('editDetail').value.trim() || null,
            price: parseInt(document.getElementById('editPrice').value),
            image: document.getElementById('editImage').value.trim() || null,
            statusId: parseInt(document.getElementById('editStatusId').value)
        };

        // Validation
        if (!itemData.itemName) {
            showAlert('error', 'Item name is required');
            return;
        }

        if (itemData.price < 0) {
            showAlert('error', 'Price must be non-negative');
            return;
        }

        showSaving();

        const response = await fetch(`/ItemManage/UpdateItem?id=${itemData.itemId}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify(itemData)
        });

        const result = await response.json();

        if (result.success) {
            showAlert('success', 'Item updated successfully');
            bootstrap.Modal.getInstance(document.getElementById('editItemModal')).hide();
            
            // Reload item details
            await loadItemDetails();
        } else {
            showAlert('error', result.message || 'Failed to update item');
        }
    } catch (error) {
        console.error('Error updating item:', error);
        showAlert('error', 'Error updating item: ' + error.message);
    } finally {
        hideSaving();
    }
}

function previewEditImage(imageUrl) {
    const preview = document.getElementById('editImagePreview');
    const container = document.getElementById('editImagePreviewContainer');
    
    if (imageUrl && imageUrl.trim()) {
        preview.src = imageUrl;
        container.style.display = 'block';
        
        preview.onerror = function() {
            hideEditImagePreview();
            showAlert('warning', 'Failed to load image from URL');
        };
    } else {
        hideEditImagePreview();
    }
}

function hideEditImagePreview() {
    document.getElementById('editImagePreviewContainer').style.display = 'none';
}

function showSaving() {
    const saveBtn = document.querySelector('#editItemModal .btn-primary');
    saveBtn.disabled = true;
    saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';
}

function hideSaving() {
    const saveBtn = document.querySelector('#editItemModal .btn-primary');
    saveBtn.disabled = false;
    saveBtn.innerHTML = 'Save Changes';
}

function showLoading() {
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('itemDetailsCard').style.display = 'none';
    document.getElementById('errorMessage').style.display = 'none';
}

function hideLoading() {
    document.getElementById('loadingSpinner').style.display = 'none';
}

function showError(message) {
    document.getElementById('errorText').textContent = message;
    document.getElementById('errorMessage').style.display = 'block';
    document.getElementById('itemDetailsCard').style.display = 'none';
}

function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
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

// Print functionality
function printItemDetails() {
    if (!currentItem) return;
    
    const printWindow = window.open('', '_blank');
    const printContent = `
        <!DOCTYPE html>
        <html>
        <head>
            <title>Item Details - ${currentItem.itemName}</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { border-bottom: 2px solid #333; padding-bottom: 10px; margin-bottom: 20px; }
                .item-info { margin: 10px 0; }
                .price { font-size: 1.2em; font-weight: bold; color: #27ae60; }
                .status { padding: 5px 10px; border-radius: 5px; background: #ddd; }
                .status.active { background: #d4edda; color: #155724; }
                .status.inactive { background: #f8d7da; color: #721c24; }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>Item Details</h1>
                <p>Generated on: ${new Date().toLocaleDateString('vi-VN')}</p>
            </div>
            <div class="item-info">
                <h2>${currentItem.itemName}</h2>
                <p><strong>Item ID:</strong> ${currentItem.itemId}</p>
                <p><strong>Price:</strong> <span class="price">${formatPrice(currentItem.price)}</span></p>
                <p><strong>Status:</strong> <span class="status ${currentItem.statusId === 1 ? 'active' : 'inactive'}">${currentItem.statusName || (currentItem.statusId === 1 ? 'Active' : 'Inactive')}</span></p>
                <p><strong>Description:</strong></p>
                <p>${currentItem.detail || 'No description available'}</p>
            </div>
        </body>
        </html>
    `;
    
    printWindow.document.write(printContent);
    printWindow.document.close();
    printWindow.print();
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

function getImagePath(imagePath) {
    if (!imagePath) {
        return '/images/default-item.jpg';
    }
    
    // If already starts with /images/, return as is
    if (imagePath.startsWith('/images/')) {
        return imagePath;
    }
    
    // If starts with http:// or https://, return as is (external URL)
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
        return imagePath;
    }
    
    // If starts with /, return as is (full path)
    if (imagePath.startsWith('/')) {
        return imagePath;
    }
    
    // Extract just filename if it's a full path
    const filename = imagePath.split('\\').pop().split('/').pop();
    
    // Handle default image fallback
    if (filename === 'default.jpg' || !filename) {
        return '/images/default-item.jpg';
    }
    
    // Known missing images - return default immediately
    const missingImages = [
        'Screenshot 2025-07-04 140208.png',
        'Screenshot 2025-07-04 140312.png',
        'Screenshot 2025-07-05 164534.png',
        'Screenshot 2025-07-04 140333.png',
        'Untitled Diagram-Page-9 (1).jpg'
    ];
    
    if (missingImages.includes(filename)) {
        return '/images/default-item.jpg';
    }
    
    // Return with /images/ prefix
    return '/images/' + filename;
}

function handleImageError(img) {
    // Prevent infinite loop if default image also fails
    if (img.src.includes('default-item.jpg')) {
        return;
    }
    
    // Silently fallback to default image without console warning
    img.src = '/images/default-item.jpg';
    img.alt = 'Default Item Image';
}
