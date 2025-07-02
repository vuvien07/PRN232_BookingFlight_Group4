let token = localStorage.getItem('flightCheckoutToken');
let flightCheckoutRequestForm = parseJwtToken(token);
flightCheckoutRequestForm = JSON.parse(flightCheckoutRequestForm.data);
async function loadPageInfo() {
    try {
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(flightCheckoutRequestForm)
        });
        if (res.ok) {
            let data = await res.json();
            flightCheckoutRequestForm.TotalAmount = data.totalPrice;
            updateInvoice(data);
        }
    } catch (err) {
        console.log(err);
        showSnackbar("Có lỗi xảy ra", "error");
    }
}

function updateFlightInfo() {
    document.getElementById('flight-info').innerHTML = `
     <div>
                                <strong class="d-block">
                                    ${flightCheckoutRequestForm?.Flight?.PlaneCode ? flightCheckoutRequestForm?.Flight?.PlaneCode : ""}
                                </strong>
                                <small>Mã CB: VJ120</small>
                            </div>
                            <div class="text-end">
                                <strong>
                                    ${flightCheckoutRequestForm?.Flight?.FromCode}
                                    → ${flightCheckoutRequestForm?.Flight?.ToCode}
                                </strong>
                                 <div>
                                     ${flightCheckoutRequestForm?.Flight?.DepartureDate ? flightCheckoutRequestForm?.Flight?.DepartureDate : ""}
                                    | ${flightCheckoutRequestForm?.Flight?.DepartureTime ? flightCheckoutRequestForm?.Flight?.DepartureTime : ""}
                                    - ${flightCheckoutRequestForm?.Flight?.ArrivalTime ? flightCheckoutRequestForm?.Flight?.ArrivalTime : ""}
                                </div>
                            </div>
    `;
}

function updateServiceSection() {
    let contentHtml = ``;
    flightCheckoutRequestForm?.Services?.map(service => {
        contentHtml += `
         <div class="flight-info">
                                <div class="align-items-center">
                                    <div class="service-card">
                                        <div class="d-flex align-items-start">
                                            <div class="flex-grow-1">
                                                <h5>
                                                    ${service.ServiceName}
                                                </h5>
                                                <p class="mb-2 selected-item-notice-fd">
                                                    Các mặt hàng<sup class="text-danger font-weight-bold">*</sup>
                                                </p>
                                            </div>
                                        </div>
        `;
        service?.Items?.forEach(item => {
            contentHtml += `
            <div class="card d-flex mb-2 flex-row w-100">
                                                <div class="card-body text-start">
                                                    <h5 class="card-title">
                                                        ${item.ItemName}
                                                    </h5>
                                                    <p class="card-text">
                                                        ${item.Detail}
                                                    </p>
                                                    <p class="card-text"><strong>Số lượng:</strong> ${item.Quantity}</p>
                                                </div>
                                            </div>
            `;
        });
        contentHtml += `
                                    </div>
                                </div>
                                </div>
                                `;
    });
    document.getElementById('service-list-info').innerHTML = contentHtml;
}
function updateInvoice(data) {
    document.getElementById('summary-card').innerHTML = `
                    <div class="summary-item">
                        <span>Giá vé</span>
                        <span class="price flight-price">${data.totalFlightPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Thuế và lệ phí</span>
                        <span class="price tax-price">${data.totalTax?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Tổng giá trị vé</span>
                        <span class="price total-flight-price">${data.totalFlightPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>
                    <div class="summary-item">
                        <span>Tổng giá trị ghế ngồi</span>
                        <span class="price seat-price">${data.totalSeatPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Phí dịch vụ</span>
                        <span class="price service-price">${data.totalServicePrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="divider"></div>

                    <div class="summary-item">
                        <strong>Tổng tiền<sup class="text-danger font-weight-bold">*</sup></strong>
                        <span class="price amount-total">${data.totalPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>`;

}

function updatePassengerInfo() {
    let contentHtml = ``;
    Array.isArray(flightCheckoutRequestForm?.PassengerInformationForms) && flightCheckoutRequestForm?.PassengerInformationForms?.length > 0 &&    flightCheckoutRequestForm?.PassengerInformationForms?.map((passenger, index) => {
        contentHtml += `
          <h5 class="mb-3">
                                    Thông tin hành khách thứ ${index + 1}<sup class="text-danger font-weight-bold">*</sup>
                                </h5>
                                <div class="card">
                                    <div class="card-body">
                                        <p class="card-text">
                                            Tên: ${passenger.Name}
                                        </p>
                                        <p class="card-text">
                                            Họ tên: ${passenger.FullName}
                                        </p>
                                        <p class="card-text">
                                            Giới tính: ${passenger.Gender}
                                        </p>
                                        ${passenger.Cccd && `
                                            <p class="card-text">
                                                Số căn cước: ${passenger.Cccd}
                                            </p>`
                                        }
                                    </div>
                                </div>
        `;
    });
    document.getElementById('passenger-info').innerHTML = contentHtml;

}

function updateContactInfo() {
    document.getElementById('contact-info').innerHTML = `
     <div class="card"
                            <div class="card-body">
                                <p class="card-text">
                                    Họ và tên: ${flightCheckoutRequestForm?.FullNameContact}
                                </p>
                                <p class="card-text">
                                    Số điện thoại: ${flightCheckoutRequestForm?.PhoneContact}
                                </p>
                                <p class="card-text">
                                    Email: ${flightCheckoutRequestForm?.EmailContact}
                                </p>
                                <p class="card-text">
                                    Địa chỉ: ${flightCheckoutRequestForm?.AddressContact}
                                </p>
                            </div>
                        </div>
       `;
}

window.onload = async () => {
    await loadPageInfo();
    updateFlightInfo();
    updateServiceSection();
    updatePassengerInfo();
    updateContactInfo();

};

async function nextToPaymentWithVnpay(e) {
    e.preventDefault();
    let paymentInfo = {
        OrderType: 'billpayment',
        Amount: flightCheckoutRequestForm?.TotalAmount,
        OrderDescription: 'Thanh toán cho chuyến bay ' + flightCheckoutRequestForm?.FlightId,
        Name:'Thanh toán'
    }
    try {
        const res = await fetch(`http://${host}:5077/api/VnPay/create-payment-url`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(paymentInfo)
        });
        if (res.ok) {
            let data = await res.json();
            window.location.href = data.url;
        }
    } catch (e) {
        showSnackbar('Error', 'error');
    }
}