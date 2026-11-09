// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Chức năng nâng cao: AJAX Live Search & Filter (Mục 19.6)
// Tự động tìm kiếm tức thì với Debounce khi gõ từ khóa, tự động lọc khi đổi dropdown, phân trang ngầm qua AJAX không tải lại trang.

(function () {
    'use strict';

    const form = document.getElementById('formBoLocSan');
    const inputTuKhoa = document.getElementById('inputTuKhoa');
    const selectLoaiSan = document.getElementById('selectLoaiSan');
    const selectKhuVuc = document.getElementById('selectKhuVuc');
    const selectTinhTrang = document.getElementById('selectTinhTrang');
    const selectSortOrder = document.getElementById('selectSortOrder');
    const btnXoaBoLoc = document.getElementById('btnXoaBoLoc');
    const vungDanhSach = document.getElementById('vungDanhSachSan');

    if (!form || !vungDanhSach) return;

    let debounceTimer = null;

    // Hàm gọi AJAX cập nhật danh sách sân
    function taiDanhSachSan(url, pushState = true) {
        // Hiển thị hiệu ứng tải
        vungDanhSach.style.opacity = '0.55';
        const spinner = document.getElementById('ajaxSpinner');
        if (spinner) spinner.classList.remove('d-none');

        fetch(url, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.text();
        })
        .then(html => {
            // Thay thế nội dung vùng danh sách
            vungDanhSach.innerHTML = html;
            vungDanhSach.style.opacity = '1';

            // Cập nhật URL trên thanh địa chỉ trình duyệt để tiện bookmark/refresh
            if (pushState) {
                window.history.pushState(null, '', url);
            }
        })
        .catch(err => {
            console.error('Lỗi khi tải danh sách sân qua AJAX:', err);
            vungDanhSach.style.opacity = '1';
            const spinner = document.getElementById('ajaxSpinner');
            if (spinner) spinner.classList.add('d-none');
        });
    }

    // Hàm xây dựng URL từ các ô input của Form
    function buildFilterUrl(pageIndex = 1) {
        const formData = new FormData(form);
        const params = new URLSearchParams();

        for (const [key, value] of formData.entries()) {
            if (value && value.toString().trim() !== '') {
                params.append(key, value.toString().trim());
            }
        }
        if (pageIndex > 1) {
            params.set('pageIndex', pageIndex.toString());
        }

        const queryString = params.toString();
        return form.action + (queryString ? '?' + queryString : '');
    }

    // 1. Debounce Live Search cho ô gõ từ khóa (350ms sau khi ngừng gõ thì tìm)
    if (inputTuKhoa) {
        inputTuKhoa.addEventListener('input', function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                const url = buildFilterUrl(1);
                taiDanhSachSan(url);
            }, 350);
        });
    }

    // 2. Tự động lọc tức thì khi thay đổi dropdown Loại sân, Khu vực, Tình trạng, Sắp xếp
    [selectLoaiSan, selectKhuVuc, selectTinhTrang, selectSortOrder].forEach(el => {
        if (el) {
            el.addEventListener('change', function () {
                const url = buildFilterUrl(1);
                taiDanhSachSan(url);
            });
        }
    });

    // 3. Xử lý khi ấn nút submit Form
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        const url = buildFilterUrl(1);
        taiDanhSachSan(url);
    });

    // 4. Xử lý click nút "Xóa" bộ lọc: Reset toàn bộ form về mặc định và tải lại
    if (btnXoaBoLoc) {
        btnXoaBoLoc.addEventListener('click', function () {
            form.reset();
            if (inputTuKhoa) inputTuKhoa.value = '';
            if (selectLoaiSan) selectLoaiSan.value = '';
            if (selectKhuVuc) selectKhuVuc.value = '';
            if (selectTinhTrang) selectTinhTrang.value = '';
            if (selectSortOrder) selectSortOrder.value = '';

            const url = form.action;
            taiDanhSachSan(url);
        });
    }

    // 5. Bắt sự kiện click các liên kết phân trang AJAX (Event Delegation)
    vungDanhSach.addEventListener('click', function (e) {
        const link = e.target.closest('a.ajax-page-link');
        if (link && link.getAttribute('href')) {
            e.preventDefault();
            const url = link.getAttribute('href');
            taiDanhSachSan(url);
            // Cuộn mượt lên đầu bảng
            window.scrollTo({ top: vungDanhSach.offsetTop - 100, behavior: 'smooth' });
        }
    });

    // 6. Hỗ trợ nút Back / Forward của trình duyệt
    window.addEventListener('popstate', function () {
        taiDanhSachSan(window.location.href, false);
    });
})();
