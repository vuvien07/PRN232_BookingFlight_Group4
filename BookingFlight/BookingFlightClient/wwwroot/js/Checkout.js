let token = localStorage.getItem('flightCheckoutToken');
let errorLabels = [];
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
            updateInvoice(data);
        }
    } catch (err) {
        console.log(err);
        showSnackbar("Có lỗi xảy ra", "error");
    }
}
window.onload = async () => {
    await loadPageInfo();
    showPassengerForm();
    loadFlightInfo();
};

function updateInvoice(data) {
    let contentHtml = ``;
    contentHtml += `
     <h5 class="mb-4">Chi tiết giá</h5>

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
                        <strong>Tổng tiền</strong>
                        <span class="price amount-total">${data.totalPrice?.toLocaleString("vi-VN")} VND</span>
                    </div>

                    <div class="mt-4">
                        <button class="btn btn-confirm" onclick="nextToConfirmPassengerForm(event)">
                            Xác nhận đặt vé
                        </button>
                        <p class="small text-muted mt-2 text-center">
                            Bằng việc xác nhận đặt vé, bạn đồng ý với các điều khoản và điều kiện của chúng tôi
                        </p>
                    </div>
    `;
    document.getElementById('invoice').innerHTML = contentHtml;
}

function loadFlightInfo() {
    let contentHtml = ``;
    contentHtml += `
     <div>
                                <strong class="d-block">
                                    ${flightCheckoutRequestForm?.Flight?.PlaneCode}
                                </strong>
                                <small>Mã CB: VJ120</small>
                            </div>
                            <div class="text-end">
                                <strong>
                                     ${flightCheckoutRequestForm?.Flight?.FromCode}
                                    →  ${flightCheckoutRequestForm?.Flight?.ToCode}
                                </strong>
                                <div>
                                     ${flightCheckoutRequestForm?.Flight?.DepartureDate ? flightCheckoutRequestForm?.Flight?.DepartureDate : ""}
                                    | ${flightCheckoutRequestForm?.Flight?.DepartureTime ? flightCheckoutRequestForm?.Flight?.DepartureTime : ""}
                                    - ${flightCheckoutRequestForm?.Flight?.ArrivalTime ? flightCheckoutRequestForm?.Flight?.ArrivalTime : ""}
                                </div>
                            </div>
    `;
    document.getElementById('flight-info').innerHTML = contentHtml;
}

function showPassengerForm() {
    let contentHtml = ``;
    flightCheckoutRequestForm.PreorderFlights?.forEach(preorder => {
        if (preorder.Name === "Adult" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                flightCheckoutRequestForm.PassengerInformationForms?.push({
                    Id: i,
                    Name: "",
                    Gender: "",
                    FullName: "",
                    DateOfBirth: "",
                    Cccd: "",
                    Type:"Adult"
                });
                errorLabels.push(`FullNameAdult${i}`);
                errorLabels.push(`NameAdult${i}`);
                errorLabels.push(`DateOfBirthAdult${i}`);
                contentHtml += `
                 <div class="checkout-section">
                                    <h5 class="mb-3">
                                        Thông tin hành khách ${i + 1}<sup class="text-danger font-weight-bold">*</sup>
                                    </h5>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-adult-${i}" id="male" value="male"
                                        onchange="handleChangeValue(adultInfoList, ${i}, 'gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Quý ông</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-adult-${i}" id="female" value="female"
                                        onchange="handleChangeValue(adultInfoList, ${i}, 'gender', this.value)">
                                        <label class="form-check-label" for="female">Quý bà</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-adult-${i}"
                                    onchange="handleChangeValue(adultInfoList, ${i}, 'name', this.value)">
                                    <p class="text-danger" id="NameAdult${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-adult-${i}"
                                    onchange="handleChangeValue(adultInfoList, ${i}, 'fullName', this.value)">
                                    <p class="text-danger" id="FullNameAdult${i}"></p>
                                    <input type="date"
                                    class="form-control"
                                    placeholder="Ngày sinh"
                                    name="dateOfBirth-adult-${i}"
                                    onchange="handleChangeValue(adultInfoList, ${i}, 'dateOfBirth', this.value)">
                                    <p class="text-danger" id="DateOfBirthAdult${i}"></p>
                                    <input type="text" class="form-control" placeholder="CCCD" name="cccd-adult"
                                    onchange="handleChangeValue(adultInfoList, ${i}, 'cccd', this.value)">
                                    <p class="text-danger" id="CccdAdult${i}"></p>
                                </div>
                `;
            }
        }
        if (preorder.Name === "Child" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                flightCheckoutRequestForm.PassengerInformationForms?.push({
                    Id: i,
                    Name: "",
                    Gender: "",
                    FullName: "",
                    DateOfBirth: "",
                    Cccd: "",
                    Type: "Child"
                });
                errorLabels.push(`FullNameChild${i}`);
                errorLabels.push(`NameChild${i}`);
                errorLabels.push(`DateOfBirthChild${i}`);
                contentHtml += `
              <div class="checkout-section">
                                    <h5 class="mb-3">
                                        Thông tin hành khách trẻ em ${i + 1}<sup class="text-danger font-weight-bold">*</sup>
                                    </h5>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-child-${i}" value="male"
                                        onchange="handleChangeValue(childInfoList, ${i}, 'gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Nam</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-child-${i}" value="female"
                                        onchange="handleChangeValue(childInfoList, ${i}, 'gender', this.value)">
                                        <label class="form-check-label" for="female">Nữ</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-child-${i}"
                                    onchange="handleChangeValue(childInfoList, ${i}, 'name', this.value)">
                                    <p class="text-danger" id="NameChild${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-child-${i}"
                                    onchange="handleChangeValue(childInfoList, ${i}, 'fullName', this.value)">
                                    <p class="text-danger" id="FullNameChild${i}"></p>
                                    <input type="date"
                                    class="form-control"
                                    placeholder="Ngày sinh"
                                    name="dateOfBirth-child-${i}"
                                    onchange="handleChangeValue(childInfoList, ${i}, 'dateOfBirth', this.value)">
                                    <p class="text-danger" id="DateOfBirthChild${i}"></p>
                                </div>
            `;
            }
        }
        if (preorder.Name === "Baby" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                flightCheckoutRequestForm.PassengerInformationForms?.push({
                    Id: i,
                    Name: "",
                    Gender: "",
                    FullName: "",
                    DateOfBirth: "",
                    Cccd: "",
                    Type: "Baby"
                });
                errorLabels.push(`FullNameBaby${i}`);
                errorLabels.push(`NameBaby${i}`);
                errorLabels.push(`DateOfBirthBaby${i}`);
                contentHtml += `
             <div class="checkout-section">
                                    <h5 class="mb-3">
                                        Thông tin hành khách em bé ${i + 1}<sup class="text-danger font-weight-bold">*</sup>
                                    </h5>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-baby-${i}" value="male"
                                               onchange="handleChangeValue(babyInfoList, ${i}, 'gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Nam</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-baby-${i}" value="female"
                                               onchange="handleChangeValue(babyInfoList, ${i}, 'gender', this.value)">
                                        <label class="form-check-label" for="female">Nữ</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-baby-${i}"
                                           onchange="handleChangeValue(babyInfoList, ${i}, 'name', this.value)">
                                    <p class="text-danger" id="NameBaby${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-baby-${i}"
                                           onchange="handleChangeValue(babyInfoList, ${i}, 'fullName', this.value)">
                                    <p class="text-danger" id="FullNameBaby${i}"></p>
                                    <input type="date"
                                           class="form-control"
                                           placeholder="Ngày sinh"
                                           name="dateOfBirth-baby-${i}"
                                           onchange="handleChangeValue(babyInfoList, ${i}, 'dateOfBirth', this.value)">
                                    <p class="text-danger" id="DateOfBirthBaby${i}"></p>
                                </div>
            `;
            }
        }
    });

    document.getElementById('passenger-info').innerHTML = contentHtml;
}

async function nextToConfirmPassengerForm(e) {
    e.preventDefault();
    try {
        const res = await fetch(`http://${host}:5077/api/CheckoutFlight/confirmPassengerForm`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(flightCheckoutRequestForm)
        });
        let result = await res.json();
        if (res.status === 400) {
            for (let i = 0; i < errorLabels.length; i++) {
                DisplayError(errorLabels[i], result);
            }
        }
    } catch (err) {
        console.log(err);
        showSnackbar("Co loi xay ra", "error");
    }
}


function DisplayError(id, errors) {
    if (!errors || !errors[id]) {
        document.getElementById(id).innerText = '';
        return;
    };
    document.getElementById(id).innerText = errors[id];
}