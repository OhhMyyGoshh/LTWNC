document.addEventListener('DOMContentLoaded', function() {
    var form = document.querySelector('form');
    if (!form) return;
    form.onsubmit = function(e) {
        var tk = document.getElementById('TaiKhoan') ? document.getElementById('TaiKhoan').value : document.getElementsByName('TaiKhoan')[0].value;
        var mk = document.getElementById('MatKhau') ? document.getElementById('MatKhau').value : document.getElementsByName('MatKhau')[0].value;
        // Validate nâng cao: chỉ báo chung "Sai mật khẩu hoặc tài khoản" nếu có lỗi
        var errorMsg = '';
        // Email
        var emailRegex = /^([\w\.-]+)@([\w\.-]+)\.\w{2,}$/;
        // Số điện thoại 10 số
        var phoneRegex = /^\d{10}$/;
        // Tài khoản: 4-50 ký tự, không ký tự đặc biệt
        var userRegex = /^\w{4,50}$/;
        var isEmail = emailRegex.test(tk);
        var isPhone = phoneRegex.test(tk);
        var isUser = userRegex.test(tk);
        if (!tk || !(isEmail || isPhone || isUser)) {
            errorMsg = 'Sai mật khẩu hoặc tài khoản';
        } else if (!mk || mk.length < 6) {
            errorMsg = 'Sai mật khẩu hoặc tài khoản';
        }
        showError('TaiKhoan', errorMsg);
        showError('MatKhau', errorMsg);
        if (errorMsg) e.preventDefault();
    };
    function showError(field, msg) {
        var el = document.getElementsByName(field)[0];
        if (!el) return;
        var span = el.parentNode.querySelector('.text-danger');
        if (span) span.innerText = msg;
    }
});
