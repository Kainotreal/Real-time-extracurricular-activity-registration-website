// Schedule Optimization JavaScript
class ScheduleOptimizer {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Kiểm tra xung đột khi tạo hoạt động
        const createActivityForm = document.getElementById('createActivityForm');
        if (createActivityForm) {
            createActivityForm.addEventListener('submit', this.handleCreateActivity.bind(this));
        }

        // Kiểm tra xung đột khi đăng ký hoạt động
        const registerButtons = document.querySelectorAll('.register-activity-btn');
        registerButtons.forEach(btn => {
            btn.addEventListener('click', this.handleRegisterActivity.bind(this));
        });

        // Kiểm tra xung đột realtime khi thay đổi thời gian
        const timeInputs = document.querySelectorAll('input[type="datetime-local"]');
        timeInputs.forEach(input => {
            input.addEventListener('change', this.handleTimeChange.bind(this));
        });
    }

    async handleCreateActivity(event) {
        event.preventDefault();
        
        const form = event.target;
        const formData = new FormData(form);
        
        try {
            // Kiểm tra xung đột trước khi submit
            const conflictResult = await this.checkScheduleConflict(formData);
            
            if (conflictResult.hasConflict) {
                this.showConflictModal(conflictResult);
                return false;
            }
            
            // Nếu không có xung đột, submit form
            form.submit();
        } catch (error) {
            console.error('Lỗi khi kiểm tra xung đột:', error);
            // Vẫn submit form nếu có lỗi
            form.submit();
        }
    }

    async handleRegisterActivity(event) {
        event.preventDefault();
        
        const button = event.target;
        const maHoatDong = button.dataset.maHoatDong;
        
        try {
            // Kiểm tra trùng lặp đăng ký
            const isDuplicate = await this.checkDuplicateRegistration(maHoatDong);
            if (isDuplicate) {
                this.showToast('Bạn đã đăng ký hoạt động này rồi!', 'warning');
                return;
            }

            // Kiểm tra xung đột lịch cá nhân
            const userConflict = await this.checkUserScheduleConflict(maHoatDong);
            if (userConflict.hasConflict) {
                this.showConflictModal(userConflict);
                return;
            }

            // Nếu không có xung đột, tiến hành đăng ký
            await this.registerActivity(maHoatDong);
        } catch (error) {
            console.error('Lỗi khi đăng ký hoạt động:', error);
            this.showToast('Có lỗi xảy ra khi đăng ký!', 'error');
        }
    }

    async handleTimeChange(event) {
        const input = event.target;
        const form = input.closest('form');
        
        if (!form) return;
        
        try {
            const formData = new FormData(form);
            const conflictResult = await this.checkScheduleConflict(formData);
            
            if (conflictResult.hasConflict) {
                this.showTimeConflictWarning(conflictResult);
            } else {
                this.hideTimeConflictWarning();
            }
        } catch (error) {
            console.error('Lỗi khi kiểm tra xung đột thời gian:', error);
        }
    }

    async checkScheduleConflict(formData) {
        const response = await fetch('/ScheduleOptimization/CheckConflict', {
            method: 'POST',
            body: formData
        });
        
        if (!response.ok) {
            throw new Error('Lỗi khi kiểm tra xung đột');
        }
        
        return await response.json();
    }

    async checkUserScheduleConflict(maHoatDong) {
        const response = await fetch(`/ScheduleOptimization/CheckUserConflict?maHoatDong=${maHoatDong}`);
        
        if (!response.ok) {
            throw new Error('Lỗi khi kiểm tra xung đột lịch cá nhân');
        }
        
        return await response.json();
    }

    async checkDuplicateRegistration(maHoatDong) {
        const response = await fetch('/ScheduleOptimization/CheckDuplicateRegistration', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.getAntiForgeryToken()
            },
            body: JSON.stringify({ maHoatDong: maHoatDong })
        });
        
        if (!response.ok) {
            throw new Error('Lỗi khi kiểm tra trùng lặp đăng ký');
        }
        
        const result = await response.json();
        return result.isDuplicate;
    }

    async registerActivity(maHoatDong) {
        const response = await fetch('/api/dangky/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.getAntiForgeryToken()
            },
            body: JSON.stringify({
                maHoatDong: maHoatDong,
                ghiChu: ''
            })
        });
        
        if (!response.ok) {
            throw new Error('Lỗi khi đăng ký hoạt động');
        }
        
        const result = await response.json();
        
        if (result.success) {
            this.showToast(result.message, 'success');
            // Reload trang hoặc cập nhật UI
            setTimeout(() => {
                window.location.reload();
            }, 1500);
        } else {
            this.showToast(result.message, 'error');
        }
    }

    showConflictModal(conflictData) {
        const modal = new bootstrap.Modal(document.getElementById('conflictModal'));
        const modalBody = document.getElementById('conflictModalBody');
        
        let content = `
            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i>
                <strong>Phát hiện xung đột lịch!</strong>
            </div>
            <div class="conflict-details">
                <h6>Chi tiết xung đột:</h6>
                <ul>
        `;
        
        conflictData.conflictMessages.forEach(message => {
            content += `<li>${message}</li>`;
        });
        
        content += `
                </ul>
            </div>
        `;
        
        if (conflictData.conflictingActivities && conflictData.conflictingActivities.length > 0) {
            content += `
                <div class="conflict-activities">
                    <h6>Hoạt động xung đột:</h6>
                    <div class="list-group">
            `;
            
            conflictData.conflictingActivities.forEach(activity => {
                content += `
                    <div class="list-group-item">
                        <h6>${activity.tieuDe}</h6>
                        <p><i class="fas fa-clock"></i> ${this.formatDateTime(activity.thoiGianBatDau)} - ${this.formatDateTime(activity.thoiGianKetThuc)}</p>
                        <p><i class="fas fa-map-marker-alt"></i> ${activity.diaDiem}</p>
                    </div>
                `;
            });
            
            content += `
                    </div>
                </div>
            `;
        }
        
        modalBody.innerHTML = content;
        modal.show();
    }

    showTimeConflictWarning(conflictData) {
        // Tạo hoặc cập nhật warning message
        let warningDiv = document.getElementById('timeConflictWarning');
        if (!warningDiv) {
            warningDiv = document.createElement('div');
            warningDiv.id = 'timeConflictWarning';
            warningDiv.className = 'alert alert-warning mt-2';
            warningDiv.innerHTML = `
                <i class="fas fa-exclamation-triangle"></i>
                <strong>Xung đột thời gian:</strong> ${conflictData.conflictMessages.join('; ')}
            `;
            
            const timeInputs = document.querySelectorAll('input[type="datetime-local"]');
            if (timeInputs.length > 0) {
                timeInputs[0].parentNode.appendChild(warningDiv);
            }
        } else {
            warningDiv.innerHTML = `
                <i class="fas fa-exclamation-triangle"></i>
                <strong>Xung đột thời gian:</strong> ${conflictData.conflictMessages.join('; ')}
            `;
            warningDiv.style.display = 'block';
        }
    }

    hideTimeConflictWarning() {
        const warningDiv = document.getElementById('timeConflictWarning');
        if (warningDiv) {
            warningDiv.style.display = 'none';
        }
    }

    async suggestTimeSlots(maHoatDong) {
        try {
            const response = await fetch(`/ScheduleOptimization/SuggestTimeSlots?maHoatDong=${maHoatDong}`);
            const suggestions = await response.json();
            
            this.showSuggestionsModal(suggestions);
        } catch (error) {
            console.error('Lỗi khi lấy đề xuất thời gian:', error);
            this.showToast('Không thể lấy đề xuất thời gian!', 'error');
        }
    }

    showSuggestionsModal(suggestions) {
        const modal = new bootstrap.Modal(document.getElementById('conflictModal'));
        const modalBody = document.getElementById('conflictModalBody');
        
        let content = `
            <div class="alert alert-info">
                <i class="fas fa-lightbulb"></i>
                <strong>Đề xuất thời gian tối ưu:</strong>
            </div>
            <div class="suggestions-list">
        `;
        
        suggestions.forEach(suggestion => {
            content += `
                <div class="suggestion-item" onclick="scheduleOptimizer.selectTimeSlot('${suggestion.startTime}', '${suggestion.endTime}', ${suggestion.diaDiemId})">
                    <span class="score-badge">${Math.round(suggestion.score)} điểm</span>
                    <strong>${suggestion.diaDiemName}</strong><br>
                    <i class="fas fa-clock"></i> ${suggestion.startTime} - ${suggestion.endTime}
                </div>
            `;
        });
        
        content += `
            </div>
        `;
        
        modalBody.innerHTML = content;
        modal.show();
    }

    selectTimeSlot(startTime, endTime, diaDiemId) {
        // Cập nhật form với thời gian được chọn
        const startInput = document.querySelector('input[name="ThoiGianBatDau"]');
        const endInput = document.querySelector('input[name="ThoiGianKetThuc"]');
        const diaDiemSelect = document.querySelector('select[name="MaDiaDiem"]');
        
        if (startInput) startInput.value = startTime;
        if (endInput) endInput.value = endTime;
        if (diaDiemSelect) diaDiemSelect.value = diaDiemId;
        
        // Đóng modal
        bootstrap.Modal.getInstance(document.getElementById('conflictModal')).hide();
        
        this.showToast('Đã cập nhật thời gian và địa điểm!', 'success');
    }

    showToast(message, type = 'info') {
        // Sử dụng toast system có sẵn hoặc tạo mới
        if (typeof showToast === 'function') {
            showToast(message, type);
        } else {
            // Tạo toast đơn giản
            const toast = document.createElement('div');
            toast.className = `toast toast-${type}`;
            toast.innerHTML = `
                <div class="toast-header">
                    <strong class="me-auto">Thông báo</strong>
                    <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body">${message}</div>
            `;
            
            const toastContainer = document.getElementById('toastContainer') || document.body;
            toastContainer.appendChild(toast);
            
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();
            
            // Tự động xóa sau 5 giây
            setTimeout(() => {
                toast.remove();
            }, 5000);
        }
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    formatDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
}

// Khởi tạo ScheduleOptimizer khi trang load
document.addEventListener('DOMContentLoaded', function() {
    window.scheduleOptimizer = new ScheduleOptimizer();
});

// Export cho sử dụng global
window.ScheduleOptimizer = ScheduleOptimizer; 