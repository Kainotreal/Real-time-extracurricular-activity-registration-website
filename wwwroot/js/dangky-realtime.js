// DangKyRealtime.js - Hỗ trợ realtime cho đăng ký hoạt động
class DangKyRealtime {
    constructor() {
        this.connection = null;
        this.currentUserId = null;
        this.currentUserRole = null;
        this.init();
    }

    async init() {
        try {
            // Khởi tạo kết nối SignalR
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubThongBao", {
                    skipNegotiation: true,
                    transport: signalR.HttpTransportType.WebSockets
                })
                .withAutomaticReconnect()
                .build();

            // Lấy thông tin user từ server
            await this.getUserInfo();

            // Thiết lập các event handlers
            this.setupEventHandlers();

            // Bắt đầu kết nối
            await this.connection.start();

            // Join các group phù hợp
            await this.joinGroups();

        } catch (error) {
            console.error("Lỗi kết nối SignalR:", error);
        }
    }

    async getUserInfo() {
        try {
            const response = await fetch('/api/user/info');
            if (response.ok) {
                const userInfo = await response.json();
                this.currentUserId = userInfo.userId;
                this.currentUserRole = userInfo.role;
            }
        } catch (error) {
            console.error("Lỗi lấy thông tin user:", error);
        }
    }

    async joinGroups() {
        try {
            if (this.currentUserRole === 'Admin') {
                await this.connection.invoke("JoinAdminGroup");
            }

            if (this.currentUserId) {
                await this.connection.invoke("JoinUserGroup", this.currentUserId);
            }
        } catch (error) {
            console.error("Lỗi join groups:", error);
        }
    }

    setupEventHandlers() {
        // Nhận thông báo chung
        this.connection.on("ReceiveNotification", (message, type) => {
            this.showNotification(message, type);
        });
    }

    showNotification(message, type = 'info') {
        try {
            // Đảm bảo message là string
            let messageString = message;
            if (typeof message === 'object') {
                console.warn("Message is object, converting to string:", message);
                messageString = JSON.stringify(message);
            } else if (typeof message !== 'string') {
                messageString = String(message);
            }
            
            // Ưu tiên sử dụng hệ thống notification chính
            if (window.realtimeNotifications && window.realtimeNotifications.isInitialized) {
                const notification = {
                    id: this.generateId(),
                    title: type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo',
                    message: messageString,
                    type: type,
                    timestamp: new Date().toLocaleTimeString('vi-VN')
                };
                window.realtimeNotifications.showNotification(notification);
            }
            // Fallback: sử dụng toast notification
            else if (typeof showToast === 'function') {
                showToast(type, messageString);
            }
            // Fallback cuối cùng: alert
            else {
                alert(`${type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo'}: ${messageString}`);
            }
        } catch (error) {
            console.error("Error showing notification:", error);
            // Ultimate fallback
            alert(`${type === 'success' ? 'Thành công' : type === 'error' ? 'Lỗi' : 'Thông báo'}: ${messageString || message}`);
        }
    }

    // Generate ID cho notification
    generateId() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    // Method để đăng ký hoạt động qua AJAX
    async registerActivity(maHoatDong, ghiChu = '') {
        try {
            const response = await fetch('/api/dangky/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    maHoatDong: maHoatDong,
                    ghiChu: ghiChu
                })
            });

            const result = await response.json();
            
            if (result.success) {
                this.showNotification(result.message, 'success');
                return true;
            } else {
                this.showNotification(result.message, 'error');
                return false;
            }
        } catch (error) {
            console.error("Lỗi đăng ký:", error);
            this.showNotification("Có lỗi xảy ra khi đăng ký", 'error');
            return false;
        }
    }

    // Method để hủy đăng ký
    async cancelRegistration(registrationId) {
        try {
            const response = await fetch(`/api/dangky/cancel/${registrationId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            const result = await response.json();
            
            if (result.success) {
                this.showNotification(result.message, 'success');
                // Reload trang hoặc cập nhật UI
                location.reload();
                return true;
            } else {
                this.showNotification(result.message, 'error');
                return false;
            }
        } catch (error) {
            console.error("Lỗi hủy đăng ký:", error);
            this.showNotification("Có lỗi xảy ra khi hủy đăng ký", 'error');
            return false;
        }
    }

    // Method để xác nhận/từ chối đăng ký (cho admin/teacher)
    async confirmRegistration(registrationId, dongY) {
        // Disable button và hiển thị loading
        const button = event.target.closest('button');
        const originalText = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';
        
        try {
            const response = await fetch(`/api/dangky/confirm/${registrationId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    dongY: dongY
                })
            });

            const result = await response.json();
            
            if (result.success) {
                this.showNotification(result.message, 'success');
                // Cập nhật UI theo thời gian thực thay vì reload trang
                this.updateConfirmationUI(registrationId, dongY);
                return true;
            } else {
                this.showNotification(result.message, 'error');
                // Restore button
                button.disabled = false;
                button.innerHTML = originalText;
                return false;
            }
        } catch (error) {
            console.error("Lỗi xác nhận đăng ký:", error);
            this.showNotification("Có lỗi xảy ra khi xác nhận đăng ký", 'error');
            // Restore button
            button.disabled = false;
            button.innerHTML = originalText;
            return false;
        }
    }

    // Cập nhật UI sau khi xác nhận/từ chối
    updateConfirmationUI(registrationId, dongY) {
        const registrationCard = document.querySelector(`[data-registration-id="${registrationId}"]`) || 
                                document.querySelector(`[data-id="${registrationId}"]`) ||
                                document.querySelector(`.registration-card[data-hoat-dong-id]`);
        
        if (registrationCard) {
            // Thêm hiệu ứng fade out
            registrationCard.style.transition = 'all 0.5s ease';
            registrationCard.style.opacity = '0';
            registrationCard.style.transform = 'scale(0.95)';
            
            setTimeout(() => {
                registrationCard.remove();
                
                // Cập nhật số lượng đăng ký chờ xác nhận
                if (typeof updateStats === 'function') {
                    updateStats();
                } else {
                    this.updatePendingCount();
                }
                
                // Kiểm tra nếu không còn đăng ký nào thì hiển thị trạng thái trống
                const remainingCards = document.querySelectorAll('.registration-card');
                if (remainingCards.length === 0) {
                    this.showEmptyState();
                }
            }, 500);
        }
    }

    // Cập nhật số lượng đăng ký chờ xác nhận
    updatePendingCount() {
        // Gọi hàm cập nhật số lượng từ trang chính nếu có
        if (typeof updateStats === 'function') {
            updateStats();
        } else {
            // Fallback: cập nhật thủ công
            const countElement = document.querySelector('.stat-item span');
            if (countElement) {
                const currentText = countElement.textContent;
                const match = currentText.match(/\d+/);
                if (match) {
                    const currentCount = parseInt(match[0]);
                    const newCount = Math.max(0, currentCount - 1);
                    countElement.textContent = `${newCount} đăng ký chờ xác nhận`;
                }
            }
        }
    }

    // Hiển thị trạng thái trống
    showEmptyState() {
        const registrationList = document.querySelector('.registration-list');
        if (registrationList) {
            registrationList.innerHTML = `
                <div class="empty-state">
                    <div class="empty-icon">
                        <i class="fas fa-check-circle"></i>
                    </div>
                    <h3>Không có đăng ký nào chờ xác nhận</h3>
                    <p>Tất cả đăng ký đã được xử lý hoặc chưa có đăng ký mới.</p>
                    <a href="/DangKyThamGia/Index" class="btn btn-primary">
                        <i class="fas fa-list"></i> Xem tất cả đăng ký
                    </a>
                </div>
            `;
        }
    }

    // Method để kiểm tra trạng thái đăng ký của user
    async checkUserRegistration(maHoatDong) {
        try {
            const response = await fetch(`/api/dangky/check/${maHoatDong}`);
            if (response.ok) {
                return await response.json();
            }
        } catch (error) {
            console.error("Lỗi kiểm tra đăng ký:", error);
        }
        return null;
    }

    // Helper method để lấy anti-forgery token
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }
}

// Khởi tạo khi trang load
document.addEventListener('DOMContentLoaded', function() {
    window.dangKyRealtime = new DangKyRealtime();
});

// Export cho sử dụng global
window.DangKyRealtime = DangKyRealtime; 