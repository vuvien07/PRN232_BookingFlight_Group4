// Add Item JavaScript

document.addEventListener('DOMContentLoaded', function() {
    setupEventListeners();
    loadStatuses();
    setupImageOptions();
});

function setupEventListeners() {
    // Form submission
    document.getElementById('addItemForm').addEventListener('submit', function(e) {
        e.preventDefault();
        saveItem();
    });

    // Image URL preview
    document.getElementById('image').addEventListener('input', function() {
        previewImage(this.value);
    });

    // Real-time validation
    document.getElementById('itemName').addEventListener('input', validateItemName);
    document.getElementById('price').addEventListener('input', validatePrice);
}

function setupImageOptions() {
    // Handle image option switching
    const imageUrlRadio = document.getElementById('imageUrl');
    const imageUploadRadio = document.getElementById('imageUpload');
    const urlContainer = document.getElementById('urlInputContainer');
    const fileContainer = document.getElementById('fileUploadContainer');
    const imageFileInput = document.getElementById('imageFile');
    const uploadActions = document.getElementById('uploadActions');
    const uploadBtn = document.getElementById('uploadBtn');
    const cancelUploadBtn = document.getElementById('cancelUploadBtn');

    imageUrlRadio.addEventListener('change', function() {
        if (this.checked) {
            urlContainer.style.display = 'block';
            fileContainer.style.display = 'none';
            resetFileUpload();
            hideImagePreview();
            // Clear uploaded image data when switching to URL mode
            document.getElementById('image').removeAttribute('data-uploaded-image');
        }
    });

    imageUploadRadio.addEventListener('change', function() {
        if (this.checked) {
            urlContainer.style.display = 'none';
            fileContainer.style.display = 'block';
            // Clear URL input but keep uploaded image data if exists
            const imageInput = document.getElementById('image');
            if (!imageInput.getAttribute('data-uploaded-image')) {
                imageInput.value = '';
            }
            hideImagePreview();
        }
    });

    // Handle file selection
    imageFileInput.addEventListener('change', function() {
        handleFileSelection(this.files);
    });

    // Handle drag and drop
    const fileUploadBtn = document.querySelector('.file-upload-btn');
    
    fileUploadBtn.addEventListener('dragover', function(e) {
        e.preventDefault();
        this.classList.add('dragover');
    });

    fileUploadBtn.addEventListener('dragleave', function(e) {
        e.preventDefault();
        this.classList.remove('dragover');
    });

    fileUploadBtn.addEventListener('drop', function(e) {
        e.preventDefault();
        this.classList.remove('dragover');
        const files = e.dataTransfer.files;
        if (files.length > 0) {
            imageFileInput.files = files;
            handleFileSelection(files);
        }
    });

    // Upload button click
    uploadBtn.addEventListener('click', function() {
        uploadImage();
    });

    // Cancel upload button
    cancelUploadBtn.addEventListener('click', function() {
        resetFileUpload();
    });
}

function handleFileSelection(files) {
    const uploadActions = document.getElementById('uploadActions');
    const fileUploadBtn = document.querySelector('.file-upload-btn');
    
    if (files && files[0]) {
        // Validate file
        const file = files[0];
        const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
        const maxSize = 5 * 1024 * 1024; // 5MB

        if (!validTypes.includes(file.type)) {
            showAlert('error', 'Please select a valid image file (JPEG, PNG, GIF)');
            resetFileUpload();
            return;
        }

        if (file.size > maxSize) {
            showAlert('error', 'File size must be less than 5MB');
            resetFileUpload();
            return;
        }

        // Update UI
        fileUploadBtn.classList.add('file-selected');
        fileUploadBtn.innerHTML = `
            <i class="fas fa-check-circle"></i>
            <div>File selected: ${file.name}</div>
            <div class="upload-info">Ready to upload (${formatFileSize(file.size)})</div>
        `;
        
        uploadActions.style.display = 'block';
        
        // Preview the selected file
        const reader = new FileReader();
        reader.onload = function(e) {
            previewImage(e.target.result);
        };
        reader.readAsDataURL(file);
    } else {
        resetFileUpload();
    }
}

function resetFileUpload() {
    const imageFileInput = document.getElementById('imageFile');
    const uploadActions = document.getElementById('uploadActions');
    const fileUploadBtn = document.querySelector('.file-upload-btn');
    const uploadProgress = document.getElementById('uploadProgress');
    
    imageFileInput.value = '';
    uploadActions.style.display = 'none';
    uploadProgress.style.display = 'none';
    
    fileUploadBtn.classList.remove('file-selected');
    fileUploadBtn.innerHTML = `
        <i class="fas fa-cloud-upload-alt"></i>
        <div>Click to select image or drag & drop</div>
        <div class="upload-info">JPEG, PNG, GIF files up to 5MB</div>
    `;
    
    // Only hide image preview if we're in upload mode and no uploaded image exists
    const imageInput = document.getElementById('image');
    if (document.getElementById('imageUpload').checked && !imageInput.getAttribute('data-uploaded-image')) {
        hideImagePreview();
    }
}

function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

async function uploadImage() {
    const fileInput = document.getElementById('imageFile');
    const uploadBtn = document.getElementById('uploadBtn');
    const uploadProgress = document.getElementById('uploadProgress');
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
        
        // Simulate progress (since fetch doesn't support upload progress easily)
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
            // Set the uploaded image URL to the image input
            const imageInput = document.getElementById('image');
            imageInput.value = result.imageUrl;
            
            // Store the uploaded image URL in a data attribute for backup
            imageInput.setAttribute('data-uploaded-image', result.imageUrl);
            
            showAlert('success', 'Image uploaded successfully!');
            console.log('Image uploaded, URL set to:', result.imageUrl);
            
            // Update preview with the uploaded image
            previewImage(result.imageUrl);
            
            // Hide upload actions
            document.getElementById('uploadActions').style.display = 'none';
            
            // Update file upload button to show success
            const fileUploadBtn = document.querySelector('.file-upload-btn');
            fileUploadBtn.innerHTML = `
                <i class="fas fa-check-circle text-success"></i>
                <div class="text-success">Image uploaded successfully!</div>
                <div class="upload-info">${fileInput.files[0].name}</div>
            `;
        } else {
            showAlert('error', result.message || 'Upload failed');
            resetFileUpload();
        }
    } catch (error) {
        console.error('Upload error:', error);
        showAlert('error', 'Upload failed: ' + error.message);
        resetFileUpload();
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

async function loadStatuses() {
    try {
        const response = await fetch('/ItemManage/GetItemStatuses', {
            headers: getAuthHeaders()
        });

        const result = await response.json();

        if (result.success) {
            const statusSelect = document.getElementById('statusId');
            statusSelect.innerHTML = result.data.map(status => 
                `<option value="${status.statusId}">${status.statusType}</option>`
            ).join('');
        }
    } catch (error) {
        console.error('Error loading statuses:', error);
    }
}

function validateItemName() {
    const itemNameInput = document.getElementById('itemName');
    const itemName = itemNameInput.value.trim();
    
    if (!itemName) {
        setFieldError(itemNameInput, 'Item name is required');
        return false;
    } else if (itemName.length < 2) {
        setFieldError(itemNameInput, 'Item name must be at least 2 characters');
        return false;
    } else if (itemName.length > 100) {
        setFieldError(itemNameInput, 'Item name must be less than 100 characters');
        return false;
    } else {
        setFieldSuccess(itemNameInput);
        return true;
    }
}

function validatePrice() {
    const priceInput = document.getElementById('price');
    const price = parseFloat(priceInput.value);
    
    if (isNaN(price) || price < 0) {
        setFieldError(priceInput, 'Price must be a non-negative number');
        return false;
    } else if (price > 999999999) {
        setFieldError(priceInput, 'Price is too large');
        return false;
    } else {
        setFieldSuccess(priceInput);
        return true;
    }
}

function setFieldError(field, message) {
    field.classList.add('is-invalid');
    field.classList.remove('is-valid');
    
    let feedback = field.parentNode.querySelector('.invalid-feedback');
    if (feedback) {
        feedback.textContent = message;
    }
}

function setFieldSuccess(field) {
    field.classList.add('is-valid');
    field.classList.remove('is-invalid');
}

async function saveItem() {
    // Validate all fields
    const isNameValid = validateItemName();
    const isPriceValid = validatePrice();

    if (!isNameValid || !isPriceValid) {
        showAlert('error', 'Please fix the validation errors before submitting');
        return;
    }

    try {
        showSaving();

        // Get image value - check for uploaded image first, then URL input
        const imageInput = document.getElementById('image');
        let imageValue = imageInput.getAttribute('data-uploaded-image') || imageInput.value.trim();
        
        // If no image provided, set to null instead of empty string
        if (!imageValue) {
            imageValue = null;
        }

        console.log('Saving item with image value:', imageValue);

        const itemData = {
            itemName: document.getElementById('itemName').value.trim(),
            detail: document.getElementById('detail').value.trim() || null,
            price: parseInt(document.getElementById('price').value),
            image: imageValue,
            statusId: parseInt(document.getElementById('statusId').value)
        };

        console.log('Item data being sent:', itemData);

        const response = await fetch('/ItemManage/CreateItem', {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(itemData)
        });

        const result = await response.json();

        if (result.success) {
            showAlert('success', 'Item created successfully!');
            setTimeout(() => {
                window.location.href = '/ItemManage/Items';
            }, 2000);
        } else {
            showAlert('error', result.message || 'Failed to create item');
        }
    } catch (error) {
        console.error('Error creating item:', error);
        showAlert('error', 'Error creating item: ' + error.message);
    } finally {
        hideSaving();
    }
}

function resetForm() {
    document.getElementById('addItemForm').reset();
    hideImagePreview();
    
    // Remove validation classes
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.classList.remove('is-valid', 'is-invalid');
    });
    
    showAlert('info', 'Form has been reset');
}

function previewImage(imageUrl) {
    const preview = document.getElementById('imagePreview');
    const container = document.getElementById('imagePreviewContainer');
    
    if (imageUrl && imageUrl.trim()) {
        preview.src = imageUrl;
        container.style.display = 'block';
        
        preview.onerror = function() {
            hideImagePreview();
            showAlert('warning', 'Failed to load image from URL');
        };
        
        preview.onload = function() {
            container.style.display = 'block';
        };
    } else {
        hideImagePreview();
    }
}

function hideImagePreview() {
    document.getElementById('imagePreviewContainer').style.display = 'none';
}

function showSaving() {
    const submitBtn = document.querySelector('button[type="submit"]');
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';
}

function hideSaving() {
    const submitBtn = document.querySelector('button[type="submit"]');
    submitBtn.disabled = false;
    submitBtn.innerHTML = '<i class="fas fa-save"></i> Save Item';
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

// Auto-format price as user types
document.addEventListener('DOMContentLoaded', function() {
    const priceInput = document.getElementById('price');
    if (priceInput) {
        priceInput.addEventListener('input', function() {
            // Remove non-numeric characters except decimal point
            let value = this.value.replace(/[^0-9]/g, '');
            
            // Format with thousand separators
            if (value) {
                const formatted = parseInt(value).toLocaleString('vi-VN');
                // Don't update if it would cause cursor issues
                if (this.value !== formatted) {
                    this.value = value; // Keep raw number for processing
                }
            }
        });
    }
});
