// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - AJAX kiểm tra trùng dữ liệu (tên đăng nhập, email, SĐT, loại sân, khu vực)
// Cách dùng: thêm data-kiem-tra="1" vào input, kèm data-url, data-param, data-truong, data-id (nếu sửa).
// Server luôn kiểm tra lại khi submit, đoạn script này chỉ giúp phản hồi sớm cho người dùng.

(function () {
    'use strict';

    var THOI_GIAN_CHO = 350; // ms, tránh gọi API liên tục khi đang gõ

    function timHopThongBao(input) {
        var chiDinh = input.getAttribute('data-target');
        if (chiDinh) {
            return document.querySelector(chiDinh);
        }
        var cha = input.parentElement;
        return cha ? cha.querySelector('[data-thong-bao]') : null;
    }

    function hienThi(input, hop, noiDung) {
        var hopThongBao = timHopThongBao(input);
        if (hopThongBao) {
            hopThongBao.textContent = noiDung || '';
            hopThongBao.className = 'small ' + (hop ? 'text-success' : 'text-danger');
        }
        input.classList.toggle('is-invalid', !hop && !!noiDung);
        input.classList.toggle('is-valid', hop && !!noiDung);
    }

    function kiemTra(input) {
        var giaTri = (input.value || '').trim();
        if (!giaTri) {
            hienThi(input, true, '');
            return;
        }

        var url = input.getAttribute('data-url');
        if (!url) {
            return;
        }

        var thamSo = new URLSearchParams();
        thamSo.set(input.getAttribute('data-param') || 'ten', giaTri);

        var truong = input.getAttribute('data-truong');
        if (truong) {
            thamSo.set('truong', truong);
        }

        var id = input.getAttribute('data-id');
        if (id) {
            thamSo.set('id', id);
        }

        fetch(url + '?' + thamSo.toString(), {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(function (phanHoi) {
                if (!phanHoi.ok) {
                    throw new Error('HTTP ' + phanHoi.status);
                }
                return phanHoi.json();
            })
            .then(function (duLieu) {
                hienThi(input, !duLieu.trung, duLieu.thongBao);
            })
            .catch(function () {
                // Mất mạng hoặc lỗi server: để server kiểm tra lại lúc submit, không chặn người dùng
                hienThi(input, true, '');
            });
    }

    document.querySelectorAll('[data-kiem-tra]').forEach(function (input) {
        var henGio = null;

        input.addEventListener('input', function () {
            clearTimeout(henGio);
            henGio = setTimeout(function () {
                kiemTra(input);
            }, THOI_GIAN_CHO);
        });

        input.addEventListener('blur', function () {
            clearTimeout(henGio);
            kiemTra(input);
        });
    });
})();
