// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function renderToast(message, type) {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = 'alert shadow-lg mb-2 alert-' + type;
    toast.innerHTML = `<span>${message}</span>`;

    container.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3000);
}

document.addEventListener('DOMContentLoaded', function () {

    document.addEventListener('showToast', function (evt) {
        const { message, type } = evt.detail;
        renderToast(message, type);
    });

});

function cartQty(initial) {
    return {
        qty: initial,
        initial: initial,
        get isChanged() {
            return this.qty !== this.initial;
        },
        increase() {
            if (this.qty < 99) this.qty++;
        },
        decrease() {
            if (this.qty > 1) this.qty--;
        }
    }
}
