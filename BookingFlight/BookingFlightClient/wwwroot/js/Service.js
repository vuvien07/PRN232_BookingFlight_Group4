
const seatTypes = {
    special: { price: 0, color: '#dc3545', name: 'Ghế đặc biệt' },
    front: { price: 0, color: '#6f42c1', name: 'Ghế hàng trước' },
    standard: { price: 0, color: '#198754', name: 'Ghế tiêu chuẩn' },
    unavailable: { price: 0, color: '#adb5bd', name: 'Không khả dụng' }
};
let token = localStorage.getItem('flightCheckoutToken');
let flightCheckoutRequestForm = token ? parseJwtToken(token) : null;
flightCheckoutRequestForm = JSON.parse(flightCheckoutRequestForm.data);
let pageInfo = null;
async function loadPageInfo() {
    try {
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(flightCheckoutRequestForm)
        });
        if (!res.ok) {
            const result = await res.json();
            if (result.message) {
                await showSnackbar(result.message, "error");
            }
        } else {
            pageInfo = await res.json();
        }
    } catch {
        showSnackbar("Có lỗi xảy ra", "error");
    }
}
window.onload = async () => {
    await loadPageInfo();
    updateServiceList(pageInfo.flightCheckoutRequestDTO.services);
    updatePrice(pageInfo);
}

function updateServiceList(data) {
    let contentHtml = ``;
    data.forEach(service => {
        contentHtml += `
        <div class="service-card">
                <div class="d-flex align-items-start">
                    <img src="${window.location.origin}/images/anh-bien.jpg" alt="Baggage" class="service-icon">
                    <div class="flex-grow-1">
                        <h5>${service.serviceName}
                        </h5>
                        <p class="mb-2">${service.detail}</p>
                        <p class="mb-2 selected-item-notice-${service.serviceId}"></p>
                        <div class="d-flex justify-content-between align-items-center">
                                <span class="text-primary" data-bs-toggle="modal"
                                      data-bs-target="#serviceModal${service.serviceId}">Xem chi tiết ></span>
                            <span class="price price-item-service"></span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade right" id="serviceModal${service.serviceId}" tabindex="-1"
                 aria-labelledby="alertModalLabel"
                 aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered modal-lg">
                    <div class="modal-content border-0 shadow-lg">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLabel">Chọn mặt hàng cho dịch
                                vụ ${service.serviceName}</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"
                                    aria-label="Close"></button>
                        </div>
                        <div class="modal-body text-center align-middle py-4">
                        ${service?.items?.map(item => `
                        <div class="card d-flex mb-2 service-card flex-row">
                                <div class="card-img w-50">
                                    <img class="card-img-top" src="${window.location.origin}/images/anh-bien.jpg" alt="#">
                                </div>
                                <div class="card-body text-start">
                                    <h5 class="card-title">${item.itemName}</h5>
                                    <p class="card-text">${item.detail}</p>
                                    <span class="price price-item-service">Giá: ${item.price} VND</span><br/>
                                    ${service.serviceId === 1 || service.serviceId === 3 ? `
                                    <div class="d-flex align-items-center">
                                    <label class="form-label price price-item-service">Số lượng: </label>
                                    <input class="w-75 form-control input-quantity-${item.itemId}" type="number" placeholder="Số lượng" min="1" >
                                    </div>
                                    ` : ''}
                                    <button class="btn btn-outline-dark ms-0 btn-select-item selectItemButton${item.itemId}"
                                            data-item='${JSON.stringify(item).replace(/"/g, "&quot;")}'
                    data-service-id="${service.serviceId}" onclick="selectItem(event)">Chọn
                                    </button>
                                    <button class="btn btn-outline-dark ms-0 btn-primary btn-select-item selectedItemButton${item.itemId}" style="display: none"  data-service-id="${service.serviceId}"
                                     data-item-id="${item.itemId}"
                                    onclick="cancelSelectItem(event)">Đã chọn
                                    </button>
                                </div>
                            </div>
                        `).join('')}
                         </div>
                    </div>
                </div>
            </div>
        `;
    });

    document.querySelector('.service-list').innerHTML = contentHtml;
}

function updatePrice(data) {
    let contentHtml = `
     <h5 class="mb-4">Chi tiết giá</h5>

                    <div class="summary-item">
                        <span>Giá vé</span>
                        <span class="price price-flight">${data.totalFlightPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Số lượng</span>
                        <span class="price num-people">${data.numberOfPeople} vé</span>
                    </div>

                    <div class="summary-item">
                        <span>Thuế, phí</span>
                        <span class="price tax-price">${data.totalTax?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Ghế ngồi:</span>
                        <span class="price seat-price">${data.totalSeatPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="summary-item">
                        <span>Dịch vụ</span>
                        <span class="price total-service-price">${data.totalServicePrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="divider"></div>

                    <div class="summary-item">
                        <strong>Tổng tiền</strong>
                        <span class="price price-total">${data.totalPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="mt-4">
                        <button class="btn btn-next" onclick="confirmServiceInfo(event)">
                            Đi tiếp
                        </button>
                    </div>
    `;
    document.getElementById('summary-card').innerHTML = contentHtml;
}

async function selectItem(event) {
    event.preventDefault();
    try {
        let itemStr = event.currentTarget.dataset.item;
        if (!itemStr) {
            showSnackbar('Không có dữ liệu item', 'error');
            return;
        }
        let item = JSON.parse(itemStr);
        let serviceId = event.currentTarget.dataset.serviceId;
        let input = document.querySelector('.input-quantity-' + item.itemId);
        if (input) {
            if (!input.value) {
                showSnackbar('Hãy nhập số lượng cho mặt hàng ' + item.itemName, 'error');
                return;
            }
            item.quantity = input.value
        }
        var service = pageInfo.flightCheckoutRequestDTO?.services?.find(s => s.serviceId === parseInt(serviceId));
        let findItem = service?.items?.find(i => i.itemId === item.itemId);
        findItem.quantity = parseInt(input.value);
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight/validate`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pageInfo.flightCheckoutRequestDTO)
        });
        if (!res.ok && res.status === 400) {
            findItem.quantity = 0;
            input.value = '';
            let result = await res.json();
            let keys = Object.keys(result);
            keys.forEach(key => {
                if (key.includes(serviceId)) {
                    showSnackbar(result[key], 'error');
                }
            });
        } else {
            let data = await res.json();
            document.querySelector('.total-service-price').textContent = `${data.totalServicePrice?.toLocaleString("vi-VN")} VND`;
            document.querySelector('.price-total').textContent = `${data.totalAmount?.toLocaleString("vi-VN")} VND`;
            document.querySelector('.selectItemButton' + item.itemId).style.display = 'none';
            document.querySelector('.selectedItemButton' + item.itemId).style.display = 'block';
            input.setAttribute('disabled', true);
            let text = 'Mặt hàng đã chọn :<br>';
            service.items.forEach(item => {
                text += item.quantity > 0 ? (item?.itemName + ' - ' + (service.serviceId !== 2 ? 'Số lượng: ' + item?.quantity + ' - ' : '') + item?.price.toLocaleString() + ' VND<br>') : '';
            });
            document.querySelector('.selected-item-notice-' + serviceId).innerHTML = text
        }
    } catch (error) {
        console.log(error);

    }
}


async function cancelSelectItem(event) {
    event.preventDefault();
    try {
        let itemId = event.currentTarget.dataset.itemId;
        let serviceId = event.currentTarget.dataset.serviceId;
        let input = document.querySelector('.input-quantity-' + itemId);
        var service = pageInfo.flightCheckoutRequestDTO?.services?.find(s => s.serviceId === parseInt(serviceId));
        let findItem = service?.items?.find(i => i.itemId === parseInt(itemId));
        findItem.quantity = 0;
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight/validate`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pageInfo.flightCheckoutRequestDTO)
        });
        let data = await res.json();
        document.querySelector('.total-service-price').textContent = `${data.totalServicePrice?.toLocaleString("vi-VN")} VND`;
        document.querySelector('.price-total').textContent = `${data.totalAmount?.toLocaleString("vi-VN")} VND`;
        document.querySelector('.selectItemButton' + itemId).style.display = 'block';
        document.querySelector('.selectedItemButton' + itemId).style.display = 'none';
        input.removeAttribute('disabled');
        input.value = '';
        let text = 'Mặt hàng đã chọn :<br>';
        service.items.forEach(item => {
            text += item.quantity > 0 ? (item?.itemName + ' - ' + (service.serviceId !== 2 ? 'Số lượng: ' + item?.quantity + ' - ' : '') + item?.price.toLocaleString() + ' VND<br>') : '';
        });
        document.querySelector('.selected-item-notice-' + serviceId).innerHTML = text
    } catch (error) {
        console.log(error);

    }
}

async function confirmServiceInfo(e) {
    e.preventDefault();
    try {
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight/confirmService`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pageInfo.flightCheckoutRequestDTO)
        });
        let data = await res.json();
        if (res.ok) {
            localStorage.setItem('flightCheckoutToken', data.token);
            window.location.href = '/flight/checkout/typeCustomerInfo';
        } else {
            showSnackbar(data, 'error');
        }
    } catch {
        showSnackbar('Có lỗi xảy ra', 'error');
    }
}
