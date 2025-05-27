window.onload = () => {
    const token = localStorage.getItem('token');
    const params = new URLSearchParams(window.location.search);
    if (!token) {
        if(params.get('isLogout')) {
            showSnackbar("Logout success", "success");
            const newUrl = `${window.location.pathname}`;
            window.history.replaceState({}, '', newUrl);
        }
        document.getElementById('headerDropdownMenu').
            innerHTML = `
            <div class="dropdown-menu" id="dropdownMenu"> 
                                  <button class="yellow-btn">Chuyến bay của tôi</button> 
                                     <button class="menu-item" onclick="signup()"> 
                                       <i class="fas fa-user-plus"></i> 
                                         Đăng ký 
                                     </button> 
                                  <button class="menu-item" onclick="login()"> 
                                        <i class="fas fa-sign-in-alt"></i> 
                                        Đăng nhập 
                                     </button> 
                                </div> 
            `;
    } else {
        showSnackbar("Login success", "success");
        let decodedToken = parseJwtToken(token);
        if (decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] == "Customer") {
            document.getElementById('headerDropdownMenu').
                innerHTML = `
            <div class="dropdown-menu" id="dropdownMenu">
                                    <button class="yellow-btn">Chuyến bay của tôi</button>
                                    <button class="menu-item" onclick="goToProfile()">
                                        <i class="fas fa-user"></i>
                                        Profile
                                    </button>

                                    <button class="menu-item" onclick="goToHistoryBooked()">
                                        <i class="fas fa-ticket"></i>
                                        Lịch sử đặt vé
                                    </button>
                                    <button class="menu-item" onclick="logout()">
                                        <i class="fas fa-sign-out-alt"></i>
                                        Đăng xuất
                                    </button>
                                </div>
            `;
        }
    }
}

document.querySelector('.yellow-btn').addEventListener('click', function (e) {
    e.preventDefault();
    var myModal = new bootstrap.Modal(document.getElementById('searchTicketModal'));
    myModal.show();
});

function toggleDropdown() {
    const dropdown = document.getElementById('dropdownMenu');
    console.log('User role:', '${sessionScope.user.role}');
    console.log('Dropdown element:', dropdown);
    dropdown.classList.toggle('show');
}

window.onclick = function (event) {
    if (!event.target.matches('.btn-outline-dark')) {
        const dropdowns = document.getElementsByClassName('dropdown-menu');
        for (let i = 0; i < dropdowns.length; i++) {
            const openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

function goToDashboard() {
    window.location.href = '/admin/dashboard';
}

function goToHistoryBooked() {
    window.location.href = '/profile/history_booked';
}

function goToProfile() {
    window.location.href = '/profile/view';
}

function signup() {
    window.location.href = '/signup';
}

function login() {
    window.location.href = '/login';
}
function logout() {
    showSnackbar("Logout success", "success");
    window.location.href = '/?isLogout=true';
    localStorage.removeItem('token');
}

function goToDashboard1() {
    window.location.href = '/manager/manage_news';
}