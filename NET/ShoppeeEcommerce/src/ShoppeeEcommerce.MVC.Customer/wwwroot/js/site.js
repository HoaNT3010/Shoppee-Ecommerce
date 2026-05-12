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

    document.body.addEventListener('htmx:afterSwap', function (e) {
        if (e.detail.target.id === "order-header") {
            const modalContainer = document.getElementById('modal-container');
            if (modalContainer) modalContainer.innerHTML = '';
        }
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

let stripe;
let elements;
let card;

async function initStripe(clientSecret) {
    const config = document.getElementById("stripe-config");
    if (!config) return;
    const publishableKey = config.dataset.publishableKey;
    const orderId = config.dataset.orderId;

    stripe = Stripe(publishableKey);
    // Payment Element requires clientSecret passed here at elements level
    elements = stripe.elements({ clientSecret });

    // Replaces card element — renders all enabled payment methods automatically
    const paymentElement = elements.create("payment", {
        layout: {
            type: "tabs",  // "tabs" | "accordion" — tabs shows card/bank/wallet as tabs
            defaultCollapsed: false,
        },
        fields: {
            // Omit billing fields if you collect them separately in your form
            billingDetails: {
                name: "auto",
                email: "auto",
                address: "auto",
            }
        },
        wallets: {
            applePay: "auto",  // shows if browser supports it
            googlePay: "auto",
        }
    });

    paymentElement.mount("#card-element"); // reuse your existing mount point

    // Show/hide pay button based on whether the element is fully filled
    paymentElement.on("change", (event) => {
        const payBtn = document.getElementById("pay-btn");
        const errorEl = document.getElementById("card-error");

        payBtn.disabled = !event.complete;

        if (event.error) {
            errorEl.textContent = event.error.message;
        } else {
            errorEl.textContent = "";
        }
    });

    document.getElementById("pay-btn").onclick = async () => {
        const payBtn = document.getElementById("pay-btn");
        const errorEl = document.getElementById("card-error");

        payBtn.disabled = true;
        payBtn.textContent = "Processing...";
        errorEl.textContent = "";

        const { error } = await stripe.confirmPayment({
            elements,
            confirmParams: {
                // Required for redirect-based methods (iDEAL, Bancontact, etc.)
                return_url: `http://localhost:5000/orders/detail/${orderId}`,

                // Pre-fill if you already have the customer's details
                payment_method_data: {
                    billing_details: {
                        name: document.getElementById("billing-name")?.value ?? "",
                        email: document.getElementById("billing-email")?.value ?? "",
                    }
                }
            },
            // Prevents redirect for methods that don't need it (e.g. card)
            // "always" | "if_required"
            redirect: "if_required"
        });

        if (error) {
            // Payment failed or was cancelled
            errorEl.textContent = error.message;
            payBtn.disabled = false;
            payBtn.textContent = "Pay Now";
        } else {
            // Payment succeeded without a redirect (e.g. card)
            htmx.ajax("GET", `http://localhost:5000/orders/detail/${orderId}`, {
                target: "body",
                swap: "outerHTML"
            });
        }
    };
}
