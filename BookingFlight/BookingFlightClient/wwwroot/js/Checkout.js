let token = localStorage.getItem('flightCheckoutToken');
let errorLabels = [];
let adultInfoList = [], childInfoList = [], infantInfoList = [], contactInfo = [];
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
    flightCheckoutRequestForm?.PreorderFlights?.map(preorder => {
        if (preorder.Name === "Adult" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                adultInfoList.push({
                    Id: i,
                    Name: "",
                    Gender: "male",
                    FullName: "",
                    DateOfBirth: "",
                    Cccd: "",
                    Type: "Adult"
                });
            }
        }
        if (preorder.Name === "Child" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                childInfoList.push({
                    Id: i,
                    Name: "",
                    Gender: "male",
                    FullName: "",
                    DateOfBirth: "",
                    Type: "Child"
                });
            }
        }
        if (preorder.Name === "Infant" && preorder.Quantity > 0) {
            for (let i = 0; i < preorder.Quantity; i++) {
                infantInfoList.push({
                    Id: i,
                    Name: "",
                    Gender: "male",
                    FullName: "",
                    DateOfBirth: "",
                    Type: "Infant"
                });
            }

        }
    });
    contactInfo.push({
        fullNameContact: '',
        phoneContact: '',
        emailContact: '',
        addressContact: ''
    })
    errorLabels.push('FullNameContact');
    errorLabels.push('PhoneContact');
    errorLabels.push('EmailContact');
    errorLabels.push('AddressContact');
};

function updateInvoice(data) {
    let contentHtml = ``;
    contentHtml += `
     <h5 class="mb-4">Chi tiết giá<sup style="color: red">*</sup></h5>

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
                        <strong>Tổng tiền<sup style="color: red">*</sup></strong>
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
                    Type: "Adult"
                });
                errorLabels.push(`FullNameAdult${i}`);
                errorLabels.push(`NameAdult${i}`);
                errorLabels.push(`DateOfBirthAdult${i}`);
                errorLabels.push(`CccdAdult${i}`);
                contentHtml += `
                 <div class="checkout-section">
                                    <h5 class="mb-3">
                                        Thông tin hành khách ${i + 1}<sup class="text-danger font-weight-bold">*</sup>
                                    </h5>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-adult-${i}" id="male" value="male"
                                        onchange="handleChangeValue('adult', ${i}, 'Gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Quý ông</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-adult-${i}" id="female" value="female"
                                        onchange="handleChangeValue('adult', ${i}, 'Gender', this.value)">
                                        <label class="form-check-label" for="female">Quý bà</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-adult-${i}"
                                    onchange="handleChangeValue('adult', ${i}, 'Name', this.value)">
                                    <p class="text-danger" id="NameAdult${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-adult-${i}"
                                    onchange="handleChangeValue('adult', ${i}, 'FullName', this.value)">
                                    <p class="text-danger" id="FullNameAdult${i}"></p>
                                    <input type="date"
                                    class="form-control"
                                    placeholder="Ngày sinh"
                                    name="dateOfBirth-adult-${i}"
                                    onchange="handleChangeValue('adult', ${i}, 'DateOfBirth', this.value)">
                                    <p class="text-danger" id="DateOfBirthAdult${i}"></p>
                                    <input type="text" class="form-control" placeholder="CCCD" name="cccd-adult-${i}"
                                    onchange="handleChangeValue('adult', ${i}, 'Cccd', this.value)">
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
                                        onchange="handleChangeValue('child', ${i}, 'Gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Nam</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-child-${i}" value="female"
                                        onchange="handleChangeValue('child', ${i}, 'Gender', this.value)">
                                        <label class="form-check-label" for="female">Nữ</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-child-${i}"
                                    onchange="handleChangeValue('child', ${i}, 'Name', this.value)">
                                    <p class="text-danger" id="NameChild${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-child-${i}"
                                    onchange="handleChangeValue('child', ${i}, 'FullName', this.value)">
                                    <p class="text-danger" id="FullNameChild${i}"></p>
                                    <input type="date"
                                    class="form-control"
                                    placeholder="Ngày sinh"
                                    name="dateOfBirth-child-${i}"
                                    onchange="handleChangeValue('child', ${i}, 'DateOfBirth', this.value)">
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
                                               onchange="handleChangeValue('infant', ${i}, 'Gender', this.value)" checked>
                                        <label class="form-check-label" for="male">Nam</label>
                                    </div>
                                    <div class="form-check mb-3">
                                        <input class="form-check-input" type="radio" name="gender-baby-${i}" value="female"
                                               onchange="handleChangeValue('infant', ${i}, 'Gender', this.value)">
                                        <label class="form-check-label" for="female">Nữ</label>
                                    </div>
                                    <input type="text" class="form-control" placeholder="Họ" name="name-baby-${i}"
                                           onchange="handleChangeValue('infant', ${i}, 'Name', this.value)">
                                    <p class="text-danger" id="NameBaby${i}"></p>
                                    <input type="text" class="form-control" placeholder="Tên đệm và tên" name="fullName-baby-${i}"
                                           onchange="handleChangeValue('infant', ${i}, 'FullName', this.value)">
                                    <p class="text-danger" id="FullNameBaby${i}"></p>
                                    <input type="date"
                                           class="form-control"
                                           placeholder="Ngày sinh"
                                           name="dateOfBirth-baby-${i}"
                                           onchange="handleChangeValue('infant', ${i}, 'DateOfBirth', this.value)">
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
        flightCheckoutRequestForm.PassengerInformationForms = adultInfoList.concat(childInfoList).concat(infantInfoList);
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
        if (res.ok) {
            localStorage.setItem('flightCheckoutToken', result.token);
            window.location.href = '/flight/checkout/confirm';
        }
    } catch (err) {
        console.log(err);
        showSnackbar("Co loi xay ra", "error");
    }
}

function handleChangeValue(type, index, field, value) {
    let list;
    if (type === 'adult') list = adultInfoList;
    else if (type === 'child') list = childInfoList;
    else if (type === 'infant') list = infantInfoList;

    list[index][field] = value;
}

function handleChangeContactValue(field, value) {
    flightCheckoutRequestForm[field] = value;
}


function DisplayError(id, errors) {
    if (!errors || !errors[id]) {
        document.getElementById(id).innerText = '';
        return;
    };
    document.getElementById(id).innerText = errors[id];
}