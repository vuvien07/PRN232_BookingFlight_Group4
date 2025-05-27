function showSnackbar(message, type) {
    return new Promise((resolve) => {
        const snackbar = document.getElementById("snackbar");
        const icon = document.getElementById("snackbarIcon");
        const text = document.getElementById("snackbarText");

        if (type === "success") {
            icon.innerHTML = '<i class="bi bi-check"></i>';
            snackbar.style.backgroundColor = "#4CAF50";
        } else if (type === "error") {
            icon.innerHTML = '<i class="bi bi-exclamation-triangle-fill"></i>';
            snackbar.style.backgroundColor = "#F28E85";
        } else {
            icon.innerHTML = '';
        }

        text.textContent = message;
        snackbar.classList.add("show");

        // Thời gian hiển thị snackbar là 3 giây
        setTimeout(() => {
            snackbar.classList.remove("show");
            resolve();  // thông báo đã xong
        }, 3000);
    });
}

