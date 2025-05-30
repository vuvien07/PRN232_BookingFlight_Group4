let checkLoadedPage = false;
let filterForm = {
    page: 1,
    pageSize: 5,
    from: '',
    to: '',
    departureDate: new Date().toISOString().split("T")[0],
    arrivalDate: '',
    departureTime: [],
    arrivalTime: [],
    brands: [],
    prices: []
}

let seatRankList = [];
let isShowDetail = false;

async function getFlightList() {
    try {
        const res = await fetch(`http://${host}:5077/api/Flight/list`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(filterForm)
        });
        if (res.ok) {
            const data = await res.json();
            let contentHtml = ``;
            if (Array.isArray(data.flights) && data.flights.length > 0) {
                for (const flight of data.flights) {
                    contentHtml += `
                                     <div class="flight-card flight-id-` + flight.flightId + `">
                            <div class="row align-items-center">
                                <div class="col-md-3">
                                    <strong>`;
                    contentHtml += flight.manufacture;
                    contentHtml += `</strong><br>`;
                    contentHtml += flight.planeCode;
                    contentHtml += `
                                </div>
                                <div class="col-md-3">
                                    <p class="mb-0">
        `;
                    contentHtml += flight.departureTime + ' - ' + flight.arrivalTime;
                    contentHtml += `</p>
                                    <small style="margin:16px;" >` + flight.fromCode + ` → ` + flight.toCode + `</small>
                                </div>
                                <div class="col-md-3">
                                    <p class="mb-0">Còn ` + flight.total + ` hạng ghế khác</p>
                                    <span class="details-btn" data-flight-id='` + flight.flightId + `' onclick="handleShowFlightDetail(event)" >Chi tiết</span>
                                </div>
                                <div class="col-md-2 text-end">
                                    <span class="price">` + flight.basePrice + ' VND' + `</span>
                                </div>
                                <div class="col-md-1 text-end">
                                    <button class ="btn btn-primary btn-sm pick-flight"
                                    data-pick-flight='` + JSON.stringify(flight) + `'
            >Chọn</button>
                                </div>
                            </div>
                        </div>`;
                }

                contentHtml += `<div class="pagination d-flex justify-content-center mt-5">
            <li class="page-item">
                <span class="page-link" data-value="1">Start</span>
            </li>`;

                contentHtml += `<li class="page-item">
            <span class="page-link" data-value="` + (data.currentPage > 1 ? data.currentPage - 1 : 1) + `">Previous</span>
        </li>`;

                for (let i = 1; i <= data.totalPage; i++) {
                    if (i === 1 || i === data.totalPage || (i >= data.currentPage - 2 && i <= data.currentPage + 2)) {
                        contentHtml += `<li class="page-item ` + (i === data.currentPage ? 'active' : '') + `">
                    <span class="page-link" data-value="` + i + `">` + i + `</span>
                </li>`;
                    } else if (i === data.currentPage - 3 || i === data.currentPage + 3) {
                        contentHtml += `<li class="page-item disabled">
                    <span class="page-link">...</span>
                </li>`;
                    }
                }

                contentHtml += `<li class="page-item">
            <span class="page-link" data-value="` + (data.currentPage < data.totalPage ? data.currentPage + 1 : data.totalPage) + `">Next</span>
        </li>`;

                contentHtml += `<li class="page-item">
            <span class="page-link" data-value="` + data.totalPage + `">End</span>
        </li>`;

                contentHtml += `</div>`;
            } else {
                contentHtml += `<p>Không tìm thấy chuyến bay </p>`
            }

            $('.flight-list-card').html(contentHtml);

            for (const flight of data.flights) {
                const flightCard = document.getElementsByClassName('flight-card flight-id-' + flight.flightId)[0];
                if (!flightCard) continue;
                await showFlightDetail(flight, flightCard);
            }

        } else {
            showSnackbar(await res.json().then(res => res.message), "error");
        }
    } catch {
        showSnackbar("Error", "error");
    }
}

// Select location
document.addEventListener('DOMContentLoaded', function () {
    let token = localStorage.getItem('flightInfoToken');
    if (!token) return;
    let searchFlightModel = parseJwtToken(token);
    const departureSelect = document.getElementById('departureSelect');
    const arrivalSelect = document.getElementById('arrivalSelect');

    filterForm.from = searchFlightModel?.From.toString();
    filterForm.to = searchFlightModel?.To.toString();

    Array.from(departureSelect.options).forEach(option => {
        option.selected = option.value === filterForm.from.toString();
    });

    Array.from(arrivalSelect.options).forEach(option => {
        option.selected = option.value === filterForm.to.toString();
    });


    // Cập nhật filterForm khi chọn điểm đi
    departureSelect.addEventListener('change', function () {
        filterForm.from = this.value;
        // Disable option đã chọn ở điểm đi trong select điểm đến
        updateArrivalOptions();
    });

    // Cập nhật filterForm khi chọn điểm đến
    arrivalSelect.addEventListener('change', function () {
        filterForm.to = this.value;
        // Disable option đã chọn ở điểm đến trong select điểm đi
        updateDepartureOptions();
    });

    // Hàm cập nhật options cho điểm đến
    function updateArrivalOptions() {
        const selectedDeparture = departureSelect.value;
        Array.from(arrivalSelect.options).forEach(option => {
            option.disabled = option.value === selectedDeparture && option.value !== '';
        });
    }

    // Hàm cập nhật options cho điểm đi
    function updateDepartureOptions() {
        const selectedArrival = arrivalSelect.value;
        Array.from(departureSelect.options).forEach(option => {
            option.disabled = option.value === selectedArrival && option.value !== '';
        });
    }
});

const dateLine = document.querySelector('.date-line');
const leftButton = document.createElement('button');
const rightButton = document.createElement('button');

leftButton.textContent = '←';
rightButton.textContent = '→';
leftButton.style.marginRight = '10px';
rightButton.style.marginLeft = '10px';

let startDate = null;
const getFormattedDate = (date) => {
    if (!(date instanceof Date) || isNaN(date)) {
        return 'Ngày không hợp lệ';
    }

    const days = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
    const months = [
        'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9',
        'Tháng 10', 'Tháng 11', 'Tháng 12'
    ];

    return `${days[date.getDay()]}<br>${date.getDate()}<br>${months[date.getMonth()]}`;
};


const createDateButtons = () => {
    dateLine.innerHTML = ''; // Clear existing buttons
    dateLine.appendChild(leftButton);

    for (let i = 0; i < 7; i++) {
        const date = new Date(startDate);
        date.setDate(startDate.getDate() + i);
        const button = document.createElement('button');
        button.innerHTML = getFormattedDate(date);
        button.dataset.index = i;

        if (i === 0) button.classList.add('active');

        dateLine.appendChild(button);
        button.addEventListener('click', async () => {
            document.querySelectorAll('.date-line button').forEach((btn) => btn.classList.remove('active'));
            filterForm.departureDate = formatDate(date);
            await getFlightList();
            button.classList.add('active');
        });
    }

    dateLine.appendChild(rightButton);
};

leftButton.addEventListener('click', async () => {
    startDate.setDate(startDate.getDate() - 1);
    filterForm.departureDate = formatDate(startDate);
    await getFlightList();
    createDateButtons();
});

rightButton.addEventListener('click', async () => {
    startDate.setDate(startDate.getDate() + 1);
    filterForm.departureDate = formatDate(startDate);
    await getFlightList();
    createDateButtons();
});

async function showFlightDetail(flight, flightCard) {
    let token = localStorage.getItem('flightInfoToken');
    if (!token) return;
    let form = parseJwtToken(token);
    const res = await fetch(`http://${host}:5077/api/Flight/detail`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            flightId: flight.flightId
        })
    });
    if (res.ok) {
        flight.seatRankList = await res.json();
    }
    if (!flightCard) return;

    let detailSection = document.getElementsByClassName('flight-detail-section flight-id-' + flight.flightId)[0];
    if (!detailSection) {
        detailSection = document.createElement('div');
        detailSection.className = 'flight-detail-section flight-id-' + flight.flightId;
        detailSection.style.marginTop = '10px';
        detailSection.style.display = 'none';
        let total = 0;
        let contentHtml = ``;
        let firstPrice = 0;
        contentHtml += `
                        <div class="alert alert-info">
                        <strong class="mb-1">Chi tiết chuyến bay:</strong><br>
                        `;
        if (flight.seatRankList.details.length > 0) {
            contentHtml += `<span style="display: inline-block" class="mt-2">Hạng ghế: </span>`;
            contentHtml += `<select class="form-select mb-2 seatRankSelect` + flight.flightId + `" style="display: inline-block; width: auto;" onchange="handleChangeSeatRank(event, '` + encodeURIComponent(JSON.stringify(flight)) + `')">`;
            flight.seatRankList.details.forEach((seatRank, index) => {
                if (index === 0) firstPrice = seatRank.price;
                contentHtml += `<option value="` + seatRank.classId + `-` + seatRank.price + `">` + seatRank.className + ` - ` + seatRank.price + ` VND</option>`
            });
            contentHtml += `</select><br>`
        }
        contentHtml += `<table class="table text-center">`;
        contentHtml += `<thead>`;
        contentHtml += `<tr>`;
        contentHtml += `<th scope="col">Loại hành khách</th>`;
        contentHtml += `<th scope="col">Số lượng vé</th>`;
        contentHtml += `<th scope="col">Giá</th>`;
        contentHtml += `<th scope="col">Thuế và phí</th>`;
        contentHtml += `<th scope="col">Ghế ngồi</th>`;
        contentHtml += `<th scope="col">Tổng tiền</th>`;
        contentHtml += `</tr>`;
        contentHtml += `</thead>`;
        contentHtml += `<tbody>`;
        if (form?.NumAdult > 0) {
            contentHtml += `<tr>`;
            contentHtml += `<td>Người lớn</td>`;
            contentHtml += `<td>` + form?.NumAdult + `</td>`;
            contentHtml += `<td class="adult-flight-price-` + flight.flightId + `">` + flight.basePrice + ` VND</td>`;
            contentHtml += `<td class="adult-tax-${flight.flightId}">` + (flight.tax * 100) + `%</td>`;
            contentHtml += `<td class="seat-adult-price-` + flight.flightId + `">` + flight.seatRankList.details[0].price + `VND</td>`;
            contentHtml += `<td class="total-adult-price-` + flight.flightId + `">` + ((flight.basePrice * form?.NumAdult) + (flight.basePrice * form?.NumAdult * flight.tax) + (flight.seatRankList.details[0].price * form?.NumAdult)) + ` VND</td>`;
            contentHtml += `</tr>`;
            total += ((flight.basePrice * form?.NumAdult) + (flight.basePrice * form?.NumAdult * flight.tax) + (flight.seatRankList.details[0].price * form?.NumAdult));
        }
        if (form?.NumChild > 0) {
            contentHtml += `<tr>`;
            contentHtml += `<td>Trẻ em</td>`;
            contentHtml += `<td>` + form?.NumChild + `</td>`;
            contentHtml += `<td class="child-flight-price-` + flight.flightId + `">` + (flight.basePrice * 0.5) + ` VND</td>`;
            contentHtml += `<td class="child-tax-${flight.flightId}">` + (flight.tax * 100) + `%</td>`;
            contentHtml += `<td class="seat-child-price-` + flight.flightId + `">` + flight.seatRankList.details[0].price + `VND</td>`;
            contentHtml += `<td class="total-child-price-` + flight.flightId + `">`
                + ((flight.basePrice * form?.NumChild * 0.5) + (flight.basePrice * form?.NumChild * flight.tax * 0.5) + (flight.seatRankList.details[0].price * form?.NumChild * 0.5)) + ` VND</td>`;
            contentHtml += `</tr>`;
            total += ((flight.basePrice * form?.NumChild * 0.5) + (flight.basePrice * form?.NumChild * flight.tax * 0.5) + (flight.seatRankList.details[0].price * form?.NumChild * 0.5));
        }
        if (form?.NumInfant > 0) {
            contentHtml += `<tr>`;
            contentHtml += `<td>Em Bé</td>`;
            contentHtml += `<td>` + form?.NumInfant + `</td>`;
            contentHtml += `<td>` + 0 + ` VND</td>`;
            contentHtml += `<td class="baby-tax-${flight.flightId}">` + (flight.tax * 100) + `%</td>`;
            contentHtml += `<td class="seat-baby-price-` + flight.flightId + `">` + flight.seatRankList.details[0].price + `VND</td>`;
            contentHtml += `<td class="total-baby-price-` + flight.flightId + `">`
                + (flight.basePrice * form?.NumInfant * (flight.tax / 4)) + ` VND</td>`;
            contentHtml += `</tr>`;
            total += (flight.basePrice * form?.NumInfant * (flight.tax / 4))
        }
        contentHtml += `</tbody>`;
        contentHtml += `</table>`;
        contentHtml += `<span class="total-price-text` + flight.flightId + `" style="font-weight: bold">Tổng cộng: ` + total + ` VND </span><br>`
        contentHtml += `</div>`;
        detailSection.innerHTML = contentHtml;

        flightCard.appendChild(detailSection);
    } else {
        detailSection.classList.toggle('d-none');
    }
}

function toggleDropdown() {
    const dropdown = document.getElementById('dropdownMenu');
    dropdown.classList.toggle('show');
}

// Đóng dropdown khi click ra ngoài
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
};

function signup() {
    window.location.href = '${pageContext.request.contextPath}/signup';
}

function login() {
    window.location.href = '${pageContext.request.contextPath}/login';
}

document.addEventListener('DOMContentLoaded', function () {
    // Helper function: cập nhật mảng theo checkbox
    function updateArrayByCheckbox(array, value, checked) {
        const index = array.indexOf(value);
        if (checked && index === -1) {
            array.push(value);
        } else if (!checked && index !== -1) {
            array.splice(index, 1);
        }
    }

    document.addEventListener('change', function (event) {
        const target = event.target;

        if (target.classList.contains('departure-time')) {
            updateArrayByCheckbox(filterForm.departureDate, target.value, target.checked);
        }

        if (target.classList.contains('arrival-time')) {
            updateArrayByCheckbox(filterForm.arrivalTime, target.value, target.checked);
        }

        if (target.classList.contains('check-brand')) {
            updateArrayByCheckbox(filterForm.brands, target.value, target.checked);
        }

        if (target.classList.contains('price-range')) {
            updateArrayByCheckbox(filterForm.prices, target.value, target.checked);
        }
    });

    document.addEventListener('click', async function (event) {
        const target = event.target;

        if (target.classList.contains('page-link')) {
            event.preventDefault();
            filterForm.page = target.dataset.value;
            await getFlightList();
        }

        if (target.classList.contains('pick-flight')) {
            event.preventDefault();
            let token = localStorage.getItem('flightInfoToken');
            if (!token) return;
            const flight = JSON.parse(target.dataset.pickFlight);
            const searchFlightModel = parseJwtToken(token);
            const seatSelect = document.querySelector(`.seatRankSelect${flight.flightId}`);
            const seatPrice = seatSelect.value;
            const [classSeatIdStr, seatPriceStr] = seatPrice.split('-');

            const getTextValue = (selector) => {
                const el = document.querySelector(selector);
                return el ? parseFloat(el.textContent) : 0;
            };

            const jsonData = {
                    flight: flight,
                    seatPrice: parseFloat(seatPriceStr),
                    classSeatId: parseInt(classSeatIdStr),
                    preorderFlights: [
                        {
                            name: 'Adult',
                            quantity: parseInt(searchFlightModel?.NumAdult),
                            totalPrice: getTextValue(`.total-adult-price-${flight.flightId}`),
                            seatPrice: getTextValue(`.seat-adult-price-${flight.flightId}`),
                            tax: parseFloat(document.querySelector(`.adult-tax-${flight.flightId}`).textContent.replace('%', '')) / 100,
                            totalFlightPrice: getTextValue(`.adult-flight-price-${flight.flightId}`)
                        },
                        {
                            name: 'Child',
                            quantity: parseInt(searchFlightModel?.NumChild),
                            totalPrice: getTextValue(`.total-child-price-${flight.flightId}`),
                            seatPrice: getTextValue(`.seat-child-price-${flight.flightId}`),
                            tax: parseFloat(document.querySelector(`.child-tax-${flight.flightId}`).textContent.replace('%', '')) / 100,
                            totalFlightPrice: getTextValue(`.child-flight-price-${flight.flightId}`)
                        },
                        {
                            name: 'Baby',
                            quantity: parseInt(searchFlightModel?.NumInfant),
                            totalPrice: getTextValue(`.total-baby-price-${flight.flightId}`),
                            tax: parseFloat(document.querySelector('.baby-tax-' + flight.flightId).textContent.replace('%', '')) / 100,
                            totalFlightPrice: 0
                        }
                    ]
            };
            searchFlightModel.NumChild = parseInt(searchFlightModel.NumChild);
            searchFlightModel.NumAdult = parseInt(searchFlightModel.NumAdult);
            searchFlightModel.NumInfant = parseInt(searchFlightModel.NumInfant);
            let formData = new FormData();
            formData.append('flightCheckoutRequestDTO', JSON.stringify(jsonData));
            formData.append('flightFormDTO', JSON.stringify(searchFlightModel));

            const res = await fetch(`http://${host}:5077/api/Flight/redirect`, {
                method: 'POST',
                body: formData
            });

            if (res.ok) {
                window.location.href = '/flight/checkout/service';
            } else {
                showSnackbar(await res.json().then(res => res.message), "error");
            }
        }
    });
});


window.onload = async function () {
    let token = localStorage.getItem('flightInfoToken');
    if (!token) return;

    let searchFlightModel = parseJwtToken(token);
    if (!searchFlightModel) return;

    const departureDateInput = document.querySelector("input[name='departure-date']");
    if (searchFlightModel?.DepartureDate !== null) {
        startDate = new Date(searchFlightModel.DepartureDate);
        if (departureDateInput) {
            departureDateInput.value = searchFlightModel.DepartureDate;
        }
    } else {
        startDate = new Date();
        if (departureDateInput) {
            // ISO format without time part
            departureDateInput.value = startDate.toISOString().split("T")[0];
        }
    }

    const days = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];

    const from = document.querySelector('#departureSelect option:checked')?.textContent || '';
    const to = document.querySelector('#arrivalSelect option:checked')?.textContent || '';

    const flightTitle = document.querySelector('.flight-title');
    if (flightTitle) {
        flightTitle.innerHTML = `Chuyến bay một chiều ${from} - ${to}`;
    }

    const departureDateTitle = document.querySelector('.departureDateTitle');
    if (departureDateTitle) {
        departureDateTitle.innerHTML = `Ngày đi: ${days[startDate.getDay()]}, ${startDate.getDate()}/${startDate.getMonth() + 1}/${startDate.getFullYear()}, Tổng số: ${searchFlightModel?.Adult} người lớn, ${searchFlightModel?.Child} trẻ em, ${searchFlightModel?.Baby} em bé`;
    }

    createDateButtons();
    await getFlightList();
};

async function handleSearchFlight(event) {
    event.preventDefault();
    let value = $("input[name='departure-date']").val();
    const days = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
    if (value) {
        startDate = new Date(value);
    } else {
        startDate = new Date();
    }
    filterForm.departureDate = formatDate(startDate);
    let from = $('#departureSelect option:selected').text();
    let to = $('#arrivalSelect option:selected').text();
    if (from === '-- Chọn điểm đi --' || to === '-- Chọn điểm đến --') {
        $('.flight-title').html(`Danh sách chuyến bay`);
    } else {
        $('.flight-title').html(`Chuyến bay một chiều ` + from + ` - ` + to);
    }
    $('.departureDateTitle').html(`Ngày đi: ` + days[startDate.getDay()] + `, ` + startDate.getDate() + `/` + (startDate.getMonth() + 1) + `/` + startDate.getFullYear());
    createDateButtons();
    await getFlightList();
}

function handleChangeSeatRank(event, jsonFlight) {
    let form = localStorage.getItem('flightInfoToken');
    event.preventDefault();
    if (!form) return;
    let selectElement = event.target;
    let value = selectElement.value;
    let flight = JSON.parse(decodeURIComponent(jsonFlight));
    if (value) {
        let total = 0;
        if (form?.Adult > 0) {
            total += (flight.basePrice * form?.Adult) + (flight.basePrice * form?.Adult * flight.tax) + (parseFloat(value.split('-')[1] * form?.Adult));
            $(`.seat-adult-price-` + flight.flightId).text((value.split('-')[1] * form?.Adult) + ' VND');
            $(`.total-adult-price-` + flight.flightId).text((flight.basePrice * form?.Adult) + (flight.basePrice * form?.Adult * flight.tax) + (parseFloat(value.split('-')[1]) * form?.Adult) + ' VND');
        }
        if (form?.Child > 0) {
            total += (flight.basePrice * form?.Child * 0.5) + (flight.basePrice * form?.Child * flight.tax * 0.5) + (parseFloat(value.split('-')[1]) * 0.5 * form?.Child);
            $(`.seat-child-price-` + flight.flightId).text((value.split('-')[1] * form?.Child * 0.5) + ' VND');
            $(`.total-child-price-` + flight.flightId).text((flight.basePrice * form?.Child * 0.5) + (flight.basePrice * form?.Child * flight.tax * 0.5) + (parseFloat(value.split('-')[1]) * 0.5 * form?.Child) + ' VND');
        }
        if (form?.Baby > 0) {
            total += (flight.basePrice * form?.Baby * flight.tax / 4);
            $(`.total-baby-price-` + flight.flightId).text((flight.basePrice * form?.Baby * flight.tax / 4) + ' VND');
        }
        let text = `- Tổng cộng: ` + total + ` VND`;
        seatPrice = parseFloat(value.split('-')[1]);
        $(`.total-price-text` + flight.flightId).html(text);
    }
}

function handleShowFlightDetail(event) {
    event.preventDefault();
    let flightId = event.target.dataset.flightId; // Lấy flightId từ data attribute
    let className = `flight-detail-section flight-id-` + flightId;
    let detailSection = document.getElementsByClassName(className)[0]; // Chọn phần tử với querySelector

    if (detailSection) { // Kiểm tra nếu phần tử tồn tại
        // Toggle hiển thị: Nếu đang ẩn thì hiện, nếu đang hiện thì ẩn
        detailSection.style.display = (detailSection.style.display === 'none' || detailSection.style.display === '') ? 'block' : 'none';
    }
}
