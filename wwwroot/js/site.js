document.addEventListener("DOMContentLoaded", function () {
    const toastContainer = document.getElementById("toast-container");

    const success = document.body.getAttribute("data-success");
    const error = document.body.getAttribute("data-error");

    if (success) showToast(success, "success");
    else if (error) showToast(error, "danger");

    function showToast(message, type = "info") {
        const toast = document.createElement("div");
        toast.className = "toast show bg-light border shadow-sm rounded text-dark px-3 small";
        toast.style.minWidth = "200px";
        toast.style.maxWidth = "300px";
        toast.style.marginTop = "12px";
        toast.style.fontSize = "0.9rem";
        toast.style.borderLeft = type === 'danger' ? '5px solid #dc3545' : '5px solid #198754';
        toast.setAttribute("role", "alert");

        const iconClass = type === "success" ? "bi-check-circle-fill" : "bi-x-circle-fill";

        toast.innerHTML = `
            <div class="d-flex align-items-center justify-content-center py-2 gap-2 text-center w-100">
                <i class="bi ${iconClass}" style="font-size: 1.1rem; color: black;"></i>
                <span class="flex-grow-1">${message}</span>
            </div>
        `;

        toastContainer.appendChild(toast);
        setTimeout(() => toast.remove(), 3000);
    }
});

function validateRegisterForm() {
    const form = document.getElementById("registerForm");

    const username = form.querySelector('[name="Username"]').value.trim();
    const email = form.querySelector('[name="Email"]').value.trim();
    const password = form.querySelector('[name="Password"]').value.trim();
    const confirmPassword = form.querySelector('[name="ConfirmPassword"]').value.trim();
    const errorDiv = document.getElementById("registerError");

    errorDiv.textContent = ""; 

    if (!username || !email || !password || !confirmPassword) {
        errorDiv.textContent = "Please fill in all fields.";
        return false;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        errorDiv.textContent = "Please enter a valid email address.";
        return false;
    }

    if (password !== confirmPassword) {
        errorDiv.textContent = "Passwords do not match.";
        return false;
    }

    return true;
}

function togglePassword(fieldId, icon) {
    const input = document.getElementById(fieldId);
    if (input.type === "password") {
        input.type = "text";
        icon.classList.replace("bi-eye-fill", "bi-eye-slash-fill");
    } else {
        input.type = "password";
        icon.classList.replace("bi-eye-slash-fill", "bi-eye-fill");
    }
}
