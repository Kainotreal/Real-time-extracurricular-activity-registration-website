// Realtime Functions - Xử lý tất cả chức năng realtime
class RealtimeFunctions {
    constructor() {
        this.init();
    }

    init() {
        this.setupFormSubmissions();
        this.setupRealTimeUpdates();
        this.setupNotifications();
    }

    // Xử lý submit form realtime
    setupFormSubmissions() {
        // Form tạo hoạt động
        const createActivityForm = document.getElementById('createActivityForm');
        if (createActivityForm) {
            createActivityForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(createActivityForm, '/HoatDong/Create', 'Tạo hoạt động');
            });
        }

        // Form chỉnh sửa hoạt động
        const editActivityForm = document.getElementById('editActivityForm');
        if (editActivityForm) {
            editActivityForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(editActivityForm, '/HoatDong/Edit', 'Cập nhật hoạt động');
            });
        }

        // Form đăng ký tham gia
        const registerForm = document.getElementById('registerForm') || document.getElementById('registrationForm');
        if (registerForm) {
            registerForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(registerForm, '/DangKyThamGia/Create', 'Đăng ký tham gia');
            });
        }

        // Form xác nhận đăng ký
        const confirmRegistrationForm = document.getElementById('confirmRegistrationForm');
        if (confirmRegistrationForm) {
            confirmRegistrationForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(confirmRegistrationForm, '/DangKyThamGia/XacNhan', 'Xác nhận đăng ký');
            });
        }

        // Form xác nhận hoạt động
        const confirmActivityForm = document.getElementById('confirmActivityForm');
        if (confirmActivityForm) {
            confirmActivityForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(confirmActivityForm, '/HoatDong/XacNhanHoatDong', 'Xác nhận hoạt động');
            });
        }

        // Form hủy đăng ký
        const deleteRegistrationForm = document.getElementById('deleteRegistrationForm');
        if (deleteRegistrationForm) {
            deleteRegistrationForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.submitFormRealtime(deleteRegistrationForm, '/DangKyThamGia/Delete', 'Hủy đăng ký');
            });
        }
    }

    // Submit form realtime
    async submitFormRealtime(form, url, actionName) {
        const formData = new FormData(form);
        const submitButton = form.querySelector('button[type="submit"]');
        const originalText = submitButton.innerHTML;

        try {
            // Disable button và hiển thị loading
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';

            const response = await fetch(url, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await response.json();

            if (result.success) {
                // Hiển thị thông báo thành công
                this.showSuccessMessage(result.message);
                
                // Cập nhật UI realtime
                this.updateUIAfterSuccess(actionName, result);
                
                // Reset form nếu cần
                if (actionName.includes('Tạo') || actionName.includes('Đăng ký')) {
                    form.reset();
                }
            } else {
                this.showErrorMessage(result.message || 'Có lỗi xảy ra!');
            }
        } catch (error) {
            console.error('Error:', error);
            this.showErrorMessage('Có lỗi xảy ra khi kết nối server!');
        } finally {
            // Restore button
            submitButton.disabled = false;
            submitButton.innerHTML = originalText;
        }
    }

    // Cập nhật UI sau khi thành công
    updateUIAfterSuccess(actionName, result) {
        if (actionName.includes('Tạo hoạt động')) {
            this.updateActivityList();
        } else if (actionName.includes('Đăng ký tham gia')) {
            this.updateRegistrationStatus(result.id);
        } else if (actionName.includes('Xác nhận')) {
            this.updateConfirmationStatus(result.id);
        } else if (actionName.includes('Hủy đăng ký')) {
            this.removeRegistrationItem(result.id);
            
            // Redirect về trang hoạt động sau khi hủy thành công
            if (result.redirectUrl) {
                setTimeout(() => {
                    window.location.href = result.redirectUrl;
                }, 1500); // Chờ 1.5 giây để user thấy thông báo
            }
        }
    }

    // Cập nhật danh sách hoạt động
    updateActivityList() {
        const activityList = document.querySelector('.activity-list');
        if (activityList) {
            // Reload danh sách hoạt động
            this.loadActivityList();
        }
    }

    // Load danh sách hoạt động realtime
    async loadActivityList() {
        try {
            const response = await fetch('/HoatDong/GetActivityList');
            const html = await response.text();
            
            const activityList = document.querySelector('.activity-list');
            if (activityList) {
                activityList.innerHTML = html;
            }
        } catch (error) {
            console.error('Error loading activity list:', error);
        }
    }

    // Cập nhật trạng thái đăng ký
    updateRegistrationStatus(registrationId) {
        const statusElement = document.querySelector(`[data-registration-id="${registrationId}"]`);
        if (statusElement) {
            statusElement.innerHTML = '<span class="badge bg-warning">Chờ xác nhận</span>';
        }
    }

    // Cập nhật trạng thái xác nhận
    updateConfirmationStatus(confirmationId) {
        const statusElement = document.querySelector(`[data-confirmation-id="${confirmationId}"]`);
        if (statusElement) {
            statusElement.innerHTML = '<span class="badge bg-success">Đã xác nhận</span>';
        }
    }

    // Xóa item đăng ký khỏi danh sách
    removeRegistrationItem(registrationId) {
        const registrationElement = document.querySelector(`[data-registration-id="${registrationId}"]`);
        if (registrationElement) {
            // Thêm hiệu ứng fade out
            registrationElement.style.transition = 'opacity 0.5s ease';
            registrationElement.style.opacity = '0';
            
            setTimeout(() => {
                registrationElement.remove();
                // Cập nhật thống kê
                this.updateMyStatistics();
            }, 500);
        }
    }

    // Cập nhật thống kê đăng ký của tôi
    updateMyStatistics() {
        const items = document.querySelectorAll('#my-registrations-timeline .activity-item');
        let pending = 0, confirmed = 0, rejected = 0;

        items.forEach(item => {
            const status = parseInt(item.dataset.status);
            switch (status) {
                case 1: pending++; break;
                case 2: confirmed++; break;
                case 3: rejected++; break;
            }
        });

        const pendingElement = document.getElementById('pending-count');
        const confirmedElement = document.getElementById('confirmed-count');
        const rejectedElement = document.getElementById('rejected-count');
        const totalElement = document.getElementById('total-count');

        if (pendingElement) pendingElement.textContent = pending;
        if (confirmedElement) confirmedElement.textContent = confirmed;
        if (rejectedElement) rejectedElement.textContent = rejected;
        if (totalElement) totalElement.textContent = pending + confirmed + rejected;
    }

    // Setup realtime updates
    setupRealTimeUpdates() {
        // Cập nhật số liệu thống kê realtime
        this.updateStatisticsRealtime();
        
        // Cập nhật danh sách realtime
        this.updateListsRealtime();
    }

    // Cập nhật thống kê realtime
    updateStatisticsRealtime() {
        setInterval(async () => {
            try {
                const response = await fetch('/Admin/GetStatistics');
                const stats = await response.json();
                
                // Cập nhật các element thống kê
                this.updateStatElement('totalActivities', stats.totalActivities);
                this.updateStatElement('totalRegistrations', stats.totalRegistrations);
                this.updateStatElement('pendingRegistrations', stats.pendingRegistrations);
            } catch (error) {
                console.error('Error updating statistics:', error);
            }
        }, 30000); // Cập nhật mỗi 30 giây
    }

    // Cập nhật element thống kê
    updateStatElement(elementId, value) {
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = value;
        }
    }

    // Cập nhật danh sách realtime
    updateListsRealtime() {
        setInterval(() => {
            this.loadActivityList();
        }, 60000); // Cập nhật mỗi phút
    }

    // Setup notifications
    setupNotifications() {
        // Hiển thị thông báo thành công
        window.showSuccessMessage = (message) => {
            this.showNotification(message, 'success');
        };

        // Hiển thị thông báo lỗi
        window.showErrorMessage = (message) => {
            this.showNotification(message, 'error');
        };
    }

    // Hiển thị thông báo
    showNotification(message, type = 'info') {
        console.log("showNotification called with:", message, type);
        
        // Đảm bảo message là string
        let messageString = message;
        if (typeof message === 'object') {
            console.warn("Message is object, converting to string:", message);
            messageString = JSON.stringify(message);
        } else if (typeof message !== 'string') {
            messageString = String(message);
        }
        
        const notification = {
            id: this.generateId(),
            title: type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo',
            message: messageString,
            type: type,
            timestamp: new Date().toLocaleTimeString('vi-VN')
        };

        console.log("Created notification object:", notification);
        console.log("window.realtimeNotifications available:", !!window.realtimeNotifications);

        // Sử dụng hệ thống notification có sẵn
        if (window.realtimeNotifications && window.realtimeNotifications.isInitialized) {
            console.log("Using realtimeNotifications system");
            try {
                window.realtimeNotifications.showNotification(notification);
            } catch (error) {
                console.error("Error using realtimeNotifications:", error);
                this.showFallbackNotification(messageString, type);
            }
        } else {
            console.log("Using fallback notification system");
            // Fallback notification
            this.showFallbackNotification(messageString, type);
        }
    }

    // Fallback notification
    showFallbackNotification(message, type) {
        try {
            const alertClass = type === 'success' ? 'alert-success' : 
                              type === 'error' ? 'alert-danger' : 
                              type === 'warning' ? 'alert-warning' : 'alert-info';
            
            const alertHtml = `
                <div class="alert ${alertClass} alert-dismissible fade show" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; max-width: 400px;">
                    <strong>${type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo'}:</strong> ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            `;

            const container = document.querySelector('.alert-container') || document.body;
            container.insertAdjacentHTML('afterbegin', alertHtml);

            // Tự động ẩn sau 5 giây
            setTimeout(() => {
                const alert = container.querySelector('.alert');
                if (alert) {
                    alert.remove();
                }
            }, 5000);
        } catch (error) {
            console.error("Error showing fallback notification:", error);
            // Ultimate fallback: alert
            alert(`${type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo'}: ${message}`);
        }
    }

    // Generate ID
    generateId() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

// Khởi tạo khi trang load
document.addEventListener('DOMContentLoaded', function() {
    window.realtimeFunctions = new RealtimeFunctions();
});

// Hàm xác nhận hoạt động
function confirmActivity(activityId, dongY) {
    // Disable button và hiển thị loading
    const button = event.target.closest('button');
    const originalText = button.innerHTML;
    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
    
    // Get anti-forgery token
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    
    // Call API
    fetch('/HoatDong/XacNhanHoatDong', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token,
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: `maHoatDong=${activityId}&dongY=${dongY}`
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Đảm bảo data.message là string
            let message = data.message;
            if (typeof data.message === 'object') {
                console.warn("Data message is object, converting to string:", data.message);
                message = JSON.stringify(data.message);
            } else if (typeof data.message !== 'string') {
                message = String(data.message || 'Thành công');
            }
            
            // Show success message
            if (window.realtimeFunctions) {
                window.realtimeFunctions.showNotification(message, 'success');
            } else if (typeof showToast === 'function') {
                showToast('success', message);
            } else {
                alert(message);
            }
            
            // Remove the activity row from the table
            const row = button.closest('tr');
            row.style.transition = 'all 0.5s ease';
            row.style.opacity = '0';
            row.style.transform = 'scale(0.95)';
            setTimeout(() => {
                row.remove();
                
                // Update the count
                const countElement = document.querySelector('.stat-number');
                if (countElement) {
                    const currentCount = parseInt(countElement.textContent);
                    countElement.textContent = Math.max(0, currentCount - 1);
                }
                
                // Check if no more activities
                const remainingRows = document.querySelectorAll('.activity-row');
                if (remainingRows.length === 0) {
                    location.reload(); // Reload to show empty state
                }
            }, 500);
        } else {
            // Đảm bảo data.message là string
            let message = data.message;
            if (typeof data.message === 'object') {
                console.warn("Data message is object, converting to string:", data.message);
                message = JSON.stringify(data.message);
            } else if (typeof data.message !== 'string') {
                message = String(data.message || 'Có lỗi xảy ra khi xử lý hoạt động');
            }
            
            // Show error message
            if (window.realtimeFunctions) {
                window.realtimeFunctions.showNotification(message, 'error');
            } else if (typeof showToast === 'function') {
                showToast('error', message);
            } else {
                alert(message);
            }
            
            // Re-enable button
            button.disabled = false;
            button.innerHTML = originalText;
        }
    })
    .catch(error => {
        console.error('Error:', error);
        if (window.realtimeFunctions) {
            window.realtimeFunctions.showNotification('Có lỗi xảy ra khi kết nối đến máy chủ', 'error');
        } else if (typeof showToast === 'function') {
            showToast('error', 'Có lỗi xảy ra khi kết nối đến máy chủ');
        } else {
            alert('Có lỗi xảy ra khi kết nối đến máy chủ');
        }
        
        // Re-enable button
        button.disabled = false;
        button.innerHTML = originalText;
    });
}

// Export cho sử dụng global
window.RealtimeFunctions = RealtimeFunctions; 