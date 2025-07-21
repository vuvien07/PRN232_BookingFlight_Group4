// Custom validation configurations for jQuery Validate
$(document).ready(function() {
    // Set default options for jQuery Validate
    if (typeof $.validator !== 'undefined') {
        $.validator.setDefaults({
            errorClass: 'is-invalid',
            validClass: 'is-valid',
            errorElement: 'div',
            errorPlacement: function (error, element) {
                error.addClass('invalid-feedback');
                
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            },
            highlight: function (element, errorClass, validClass) {
                $(element).addClass(errorClass).removeClass(validClass);
                $(element).closest('.form-group').addClass('has-error');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass(errorClass).addClass(validClass);
                $(element).closest('.form-group').removeClass('has-error');
            }
        });

        // Custom validation methods
        $.validator.addMethod("strongPassword", function(value, element) {
            return this.optional(element) || 
                /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/.test(value);
        }, "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");

        $.validator.addMethod("username", function(value, element) {
            return this.optional(element) || /^[a-zA-Z0-9_]{3,50}$/.test(value);
        }, "Tên đăng nhập chỉ chứa chữ, số và dấu gạch dưới, từ 3-50 ký tự.");
    }
});
