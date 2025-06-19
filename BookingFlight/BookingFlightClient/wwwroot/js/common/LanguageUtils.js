/**
 * Language Utility Functions
 * Provides helper functions for language switching across the application
 */

// Language configuration
const LANGUAGE_CONFIG = {
    'vn': { 
        code: 'VN', 
        name: 'Tiếng Việt', 
        flag: 'vn',
        locale: 'vi-VN',
        direction: 'ltr'
    },
    'en': { 
        code: 'EN', 
        name: 'English', 
        flag: 'us',
        locale: 'en-US',
        direction: 'ltr'
    },
    'zh': { 
        code: 'ZH', 
        name: '中文', 
        flag: 'cn',
        locale: 'zh-CN',
        direction: 'ltr'
    }
};

class LanguageUtils {
    constructor() {
        this.currentLanguage = 'vn';
        this.defaultLanguage = 'vn';
        this.storageKey = 'preferred-language';
        this.eventListeners = [];
    }

    /**
     * Initialize language utility
     */
    init() {
        this.loadSavedLanguage();
        this.bindEvents();
        return this;
    }

    /**
     * Get current language
     */
    getCurrentLanguage() {
        return this.currentLanguage;
    }

    /**
     * Get language configuration
     */
    getLanguageConfig(lang = null) {
        if (lang) {
            return LANGUAGE_CONFIG[lang] || null;
        }
        return LANGUAGE_CONFIG;
    }

    /**
     * Check if language is supported
     */
    isLanguageSupported(lang) {
        return LANGUAGE_CONFIG.hasOwnProperty(lang);
    }

    /**
     * Switch to a specific language
     */
    switchLanguage(lang) {
        if (!this.isLanguageSupported(lang)) {
            console.warn('Unsupported language:', lang);
            return false;
        }

        const oldLang = this.currentLanguage;
        this.currentLanguage = lang;

        // Update DOM elements
        this.updateDOMElements(lang);
        
        // Update document attributes
        this.updateDocumentAttributes(lang);
        
        // Save preference
        this.saveLanguagePreference(lang);
        
        // Dispatch event
        this.dispatchLanguageChangeEvent(lang, oldLang);
        
        console.log(`Language switched from ${oldLang} to ${lang}`);
        return true;
    }

    /**
     * Update all DOM elements with language data attributes
     */
    updateDOMElements(lang) {        // Update text elements
        const textElements = document.querySelectorAll('[data-vn][data-en], [data-vn][data-en][data-zh]');
        textElements.forEach(element => {
            const text = element.getAttribute(`data-${lang}`);
            if (text && text.trim()) {
                this.updateElementText(element, text);
            }
        });

        // Update placeholder attributes
        const inputElements = document.querySelectorAll('input[placeholder]');
        inputElements.forEach(input => {
            const placeholderText = input.getAttribute(`data-placeholder-${lang}`);
            if (placeholderText) {
                input.placeholder = placeholderText;
            }
        });

        // Update title attributes
        const titleElements = document.querySelectorAll('[data-title-vn][data-title-en]');
        titleElements.forEach(element => {
            const title = element.getAttribute(`data-title-${lang}`);
            if (title) {
                element.title = title;
            }
        });        // Update alt attributes for images
        const imgElements = document.querySelectorAll('img[data-alt-vn][data-alt-en]');
        imgElements.forEach(img => {
            const alt = img.getAttribute(`data-alt-${lang}`);
            if (alt) {
                img.alt = alt;
            }
        });        // Update select option text
        const selectOptions = document.querySelectorAll('option[data-vn][data-en], option[data-vn][data-en][data-zh]');
        selectOptions.forEach(option => {
            const text = option.getAttribute(`data-${lang}`);
            if (text && text.trim()) {
                option.textContent = text;
            }
        });

        console.log(`Updated ${textElements.length} text elements, ${inputElements.length} input placeholders, ${selectOptions.length} select options for language: ${lang}`);
    }

    /**
     * Update element text while preserving child elements
     */
    updateElementText(element, text) {
        const children = Array.from(element.children);
        if (children.length > 0) {
            // If element has children, only update text nodes
            element.childNodes.forEach(node => {
                if (node.nodeType === Node.TEXT_NODE && node.textContent.trim()) {
                    node.textContent = text;
                }
            });
        } else {
            element.textContent = text;
        }
    }

    /**
     * Update document-level attributes
     */
    updateDocumentAttributes(lang) {
        const config = LANGUAGE_CONFIG[lang];
        if (config) {
            document.documentElement.lang = config.locale.split('-')[0];
            document.documentElement.dir = config.direction;
        }
    }

    /**
     * Load saved language preference
     */
    loadSavedLanguage() {
        const savedLang = localStorage.getItem(this.storageKey) || this.defaultLanguage;
        if (this.isLanguageSupported(savedLang)) {
            this.currentLanguage = savedLang;
        } else {
            this.currentLanguage = this.defaultLanguage;
        }
    }

    /**
     * Save language preference to localStorage
     */
    saveLanguagePreference(lang) {
        try {
            localStorage.setItem(this.storageKey, lang);
        } catch (error) {
            console.warn('Failed to save language preference:', error);
        }
    }

    /**
     * Add event listener for language changes
     */
    onLanguageChange(callback) {
        if (typeof callback === 'function') {
            this.eventListeners.push(callback);
        }
    }

    /**
     * Remove event listener
     */
    removeLanguageChangeListener(callback) {
        const index = this.eventListeners.indexOf(callback);
        if (index > -1) {
            this.eventListeners.splice(index, 1);
        }
    }

    /**
     * Dispatch language change event
     */
    dispatchLanguageChangeEvent(newLang, oldLang) {
        const config = LANGUAGE_CONFIG[newLang];
        
        // Call registered listeners
        this.eventListeners.forEach(callback => {
            try {
                callback(newLang, oldLang, config);
            } catch (error) {
                console.error('Error in language change listener:', error);
            }
        });

        // Dispatch DOM event
        window.dispatchEvent(new CustomEvent('languageChanged', {
            detail: {
                language: newLang,
                previousLanguage: oldLang,
                config: config
            }
        }));
    }

    /**
     * Bind global events
     */
    bindEvents() {
        // Listen for storage changes from other tabs
        window.addEventListener('storage', (e) => {
            if (e.key === this.storageKey && e.newValue !== this.currentLanguage) {
                this.switchLanguage(e.newValue);
            }
        });
    }

    /**
     * Get translated text for current language
     */
    getText(vnText, enText) {
        return this.currentLanguage === 'vn' ? vnText : enText;
    }

    /**
     * Format number according to current language locale
     */
    formatNumber(number, options = {}) {
        const config = LANGUAGE_CONFIG[this.currentLanguage];
        if (config && config.locale) {
            return new Intl.NumberFormat(config.locale, options).format(number);
        }
        return number.toString();
    }

    /**
     * Format date according to current language locale
     */
    formatDate(date, options = {}) {
        const config = LANGUAGE_CONFIG[this.currentLanguage];
        if (config && config.locale) {
            return new Intl.DateTimeFormat(config.locale, options).format(date);
        }
        return date.toString();
    }

    /**
     * Format currency according to current language
     */
    formatCurrency(amount, currency = 'VND') {
        const config = LANGUAGE_CONFIG[this.currentLanguage];
        const currencyOptions = {
            style: 'currency',
            currency: currency
        };

        if (config && config.locale) {
            return new Intl.NumberFormat(config.locale, currencyOptions).format(amount);
        }
        return amount.toString();
    }
}

// Create global instance
window.LanguageUtils = new LanguageUtils();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.LanguageUtils.init();
    });
} else {
    window.LanguageUtils.init();
}

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = LanguageUtils;
}
