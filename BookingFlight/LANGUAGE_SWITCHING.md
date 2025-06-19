# Chức Năng Đổi Ngôn Ngữ (Language Switching Feature)

## Tổng Quan (Overview)

Hệ thống đổi ngôn ngữ được thiết kế để hỗ trợ đa ngôn ngữ trong ứng dụng BookingFlight, hiện tại hỗ trợ Tiếng Việt và Tiếng Anh.

## Cấu Trúc Files (File Structure)

```
wwwroot/
├── js/common/
│   ├── LanguageUtils.js    # Core language utility functions
│   └── Header.js           # Header language switching integration
├── css/common/
│   └── Header.css          # Language selector styling
└── language-test.html      # Demo page for testing
```

## Cách Sử Dụng (Usage)

### 1. Trong HTML/Razor Views

Để một phần tử có thể đổi ngôn ngữ, thêm data attributes:

```html
<!-- Text elements -->
<span data-vn="Xin chào" data-en="Hello">Xin chào</span>
<h1 data-vn="Trang chủ" data-en="Home">Trang chủ</h1>

<!-- Input placeholders -->
<input type="text" placeholder="Nhập tên" 
       data-placeholder-vn="Nhập tên" 
       data-placeholder-en="Enter name">

<!-- Buttons -->
<button data-vn="Đăng nhập" data-en="Login">Đăng nhập</button>

<!-- Links -->
<a href="#" data-vn="Liên hệ" data-en="Contact">Liên hệ</a>
```

### 2. Trong JavaScript

```javascript
// Chuyển đổi ngôn ngữ
window.LanguageUtils.switchLanguage('en'); // hoặc 'vn'

// Lấy ngôn ngữ hiện tại
const currentLang = window.LanguageUtils.getCurrentLanguage();

// Lấy thông tin cấu hình ngôn ngữ
const config = window.LanguageUtils.getLanguageConfig('vn');

// Lắng nghe sự kiện đổi ngôn ngữ
window.LanguageUtils.onLanguageChange((newLang, oldLang, config) => {
    console.log(`Language changed from ${oldLang} to ${newLang}`);
});

// Hoặc sử dụng DOM event
window.addEventListener('languageChanged', function(event) {
    console.log('Language changed:', event.detail);
});
```

### 3. Helper Functions

```javascript
// Lấy text theo ngôn ngữ hiện tại
const text = window.LanguageUtils.getText('Tiếng Việt', 'English');

// Format số theo locale
const number = window.LanguageUtils.formatNumber(1234567.89);

// Format ngày tháng
const date = window.LanguageUtils.formatDate(new Date());

// Format tiền tệ
const price = window.LanguageUtils.formatCurrency(1500000, 'VND');
```

## Tính Năng (Features)

### ✅ Đã Hoàn Thành
- [x] Chuyển đổi ngôn ngữ cơ bản (VN/EN)
- [x] Lưu trữ preference trong localStorage
- [x] Tự động tải ngôn ngữ đã lưu khi refresh trang
- [x] Cập nhật text content, placeholders, titles
- [x] Event system cho language changes
- [x] Fallback mechanism khi LanguageUtils không available
- [x] Language selector UI trong header
- [x] Responsive design cho mobile
- [x] Keyboard navigation (ESC key)
- [x] Click outside để đóng dropdown
- [x] Active state cho language options
- [x] Smooth animations và transitions
- [x] Helper functions cho formatting

### 🔄 Cải Tiến Thêm (Future Enhancements)
- [ ] Thêm ngôn ngữ khác (Trung, Nhật, Hàn)
- [ ] RTL language support
- [ ] Server-side language detection
- [ ] Dynamic loading của language files
- [ ] Language-specific date/time formats
- [ ] Currency conversion
- [ ] Image/icon changes theo ngôn ngữ

## Cách Thêm Ngôn Ngữ Mới (Adding New Languages)

### 1. Cập nhật LANGUAGE_CONFIG trong LanguageUtils.js:

```javascript
const LANGUAGE_CONFIG = {
    'vn': { code: 'VN', name: 'Tiếng Việt', flag: 'vn', locale: 'vi-VN' },
    'en': { code: 'EN', name: 'English', flag: 'us', locale: 'en-US' },
    'zh': { code: 'ZH', name: '中文', flag: 'cn', locale: 'zh-CN' }, // Mới
    'ja': { code: 'JA', name: '日本語', flag: 'jp', locale: 'ja-JP' } // Mới
};
```

### 2. Thêm options trong HTML:

```html
<div class="lang-option" data-lang="zh">
    <img src="https://flagcdn.com/w20/cn.png" alt="ZH" class="flag-icon">
    <span>中文</span>
</div>
```

### 3. Thêm data attributes cho các elements:

```html
<span data-vn="Xin chào" data-en="Hello" data-zh="你好" data-ja="こんにちは">Xin chào</span>
```

## Testing

Để test chức năng đổi ngôn ngữ:

1. Mở file `wwwroot/language-test.html` trong browser
2. Click các nút "Chuyển sang Tiếng Việt" / "Switch to English"
3. Kiểm tra xem tất cả text đã đổi đúng chưa
4. Refresh trang để test localStorage persistence
5. Kiểm tra responsive design trên mobile

## Browser Support

- ✅ Chrome 60+
- ✅ Firefox 55+
- ✅ Safari 11+
- ✅ Edge 79+
- ✅ Mobile browsers

## Troubleshooting

### Vấn đề thường gặp:

1. **Text không đổi**: Kiểm tra xem element có đủ data attributes không
2. **Console errors**: Đảm bảo LanguageUtils.js được load trước Header.js
3. **Preference không lưu**: Kiểm tra localStorage có bị disable không
4. **Dropdown không hiển thị**: Kiểm tra CSS và z-index

### Debug:

```javascript
// Kiểm tra LanguageUtils
console.log(window.LanguageUtils);

// Kiểm tra ngôn ngữ hiện tại
console.log(window.LanguageUtils.getCurrentLanguage());

// Kiểm tra elements có data attributes
console.log(document.querySelectorAll('[data-vn][data-en]').length);
```

## Performance Notes

- Language switching được optimize để chỉ update elements có thay đổi
- localStorage access được minimize
- Event listeners được properly managed
- CSS transitions được optimize

## Accessibility

- Keyboard navigation support (ESC key)
- Screen reader friendly
- High contrast support
- ARIA attributes có thể được thêm trong tương lai

---

## Liên Hệ (Contact)

Nếu có vấn đề hoặc cần hỗ trợ, vui lòng tạo issue hoặc liên hệ team development.
