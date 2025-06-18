document.addEventListener("DOMContentLoaded", function () {
    // Xử lý sự kiện change trên checkbox có class 'rememberMe'
    document.addEventListener("change", function (event) {
        if (event.target && event.target.classList.contains("rememberMe")) {
            var checkbox = event.target;
            var rememberMeInput = document.getElementById("rememberMe");
            if (rememberMeInput) {
                rememberMeInput.value = checkbox.checked ? true : false;
            }
        }
    });

    // Xử lý sự kiện submit form có id 'loginForm'
    var loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", function (event) {
            var rememberCheckbox = loginForm.querySelector(".rememberMe");
            var isRememberInput = loginForm.querySelector('input[name="IsRememberMe"]');

            if (rememberCheckbox && isRememberInput) {
                isRememberInput.value = rememberCheckbox.checked ? true : false;
            }
        });
    }
});

async function LoginToSystem(e) {
    try {
        e.preventDefault();
        let labels = [
            'Username',
            'Password',]
        const res = await fetch(`http://${host}:5077/api/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "SessionId": localStorage.getItem("token")
            },
            body: JSON.stringify({
                "Username": document.querySelector('input[name="Username"]').value,
                "Password": document.querySelector('input[name="Password"]').value
            })
        });
        if (!res.ok) {
            const result = await res.json();
            if (result.errors) {
                let errors = result.errors;
                for (let i = 0; i < labels.length; i++) {
                    DisplayError(labels[i], errors);
                }
            }
            if (result.message) {
                await showSnackbar(result.message, "error");
            }
        } else {
            let json = await res.json();
            localStorage.setItem("token", json.token);
            let parseToken = parseJwtToken(json.token);
            await fetch(`http://${host}:5189/api/AfterLogin?role=${parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]}`)
                .then(res => {
                    if (!res.ok) throw new Error("Request failed");
                    return;
                })
                .then(() => {
                    window.location.href = "/";
                })
                .catch(err => {
                    console.error("AfterLogin error:", err);
                    alert("Đăng nhập thất bại hoặc role không hợp lệ.");
                });
        }
    } catch (error) {
        console.log(error);
    }
}

function DisplayError(id, errors) {
    if (!errors || !errors[id]) {
        document.getElementById(id).innerText = '';
        return;
    };
    document.getElementById(id).innerText = errors[id];
}
