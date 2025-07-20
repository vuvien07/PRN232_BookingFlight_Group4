let requestGemini = {
    prompt: '',
    isProvidedNumOfPassenger: false,
    isSearchFlight: false,
    isSelectedFlight: false,
    isSelectedClassSeat: false,
    isBookedFlight: true,
    numChild: 0,
    numAdult: 0,
    numInfant: 0,
    flightPrice: 0,
    tax: 0,
    selectedFlightId: 0,
    selectedFlightSeat: 0,
    selectedFlightSeatPrice: 0,
    flightManufacture: '',
    flightPlaneCode: '',
    flightDepartureTime: '',
    flightArrivalTime: '',
    flightFromCode: '',
    flightToCode: '',
    flightDepartureDate: '',
}
let filterFormWithGemini = {
    page: 1,
    pageSize: 10,
    from: '',
    to: '',
    departureDate: '',
    arrivalDate: '',
    departureTime: [],
    arrivalTime: [],
    brands: [],
    prices: []
}
window.addEventListener('load', function () {
    addBotMessage('Xin chào, chúng tôi là tư vấn hỗ trợ tìm kiếm chuyến bay. Chúng tôi có thể giúp gì cho bạn?');
});
let flightForm = {
    NumAdult: 0,
    NumChild: 0,
    NumInfant: 0
}

function addBotMessage(htmlContent) {
    const wrapper = document.createElement('div');
    wrapper.className = 'bot-message d-flex align-items-start mb-3';

    const icon = document.createElement('i');
    icon.className = 'fa-solid fa-robot mt-1 me-3 bot-icon';

    const messageDiv = document.createElement('div');
    messageDiv.className = 'message-text bg-bot';
    messageDiv.innerHTML = htmlContent; // <-- hỗ trợ HTML

    wrapper.appendChild(icon);
    wrapper.appendChild(messageDiv);

    document.querySelector('.chat-container').appendChild(wrapper);

    // Optional: auto scroll to bottom
    wrapper.scrollIntoView({ behavior: 'smooth' });
}

function addUserMessage(text) {
    const wrapper = document.createElement('div');
    wrapper.className = 'user-message mb-2';

    const messageDiv = document.createElement('div');
    messageDiv.className = 'message-text bg-user';
    messageDiv.textContent = text;

    wrapper.appendChild(messageDiv);
    document.querySelector('.chat-container').appendChild(wrapper);
    wrapper.scrollIntoView({ behavior: 'smooth' });
}

async function askAI(event) {
    event.preventDefault();
    const userMessage = document.querySelector('input[name="userPrompt"]').value;
    requestGemini.prompt = userMessage;
    if (userMessage) {
        addUserMessage(userMessage);
        document.querySelector('input[name="userPrompt"]').value = '';
    }
    const res = await fetch(`http://${host}:5077/api/gemini/ask`, {
        method: "POST",
        headers: { "Content-Type": "application/json" }
        , body: JSON.stringify(requestGemini),
        credentials: 'include'
    });
    if (res.ok) {
        const aiText = await res.text();
        addBotMessage(formatFlightTextToHTML(aiText));
        let isTriggeredBooking = false;

        for (const split of aiText.split('\n')) {
            const parts = split.trim().split(/\s+/);

            if (split.includes('Số lượng người lớn')) {
                requestGemini.numAdult = parts.length >= 6 ? parseInt(parts[5]) : 0;
                requestGemini.isProvidedNumOfPassenger = true;
            }

            if (split.includes('Số lượng trẻ em')) {
                requestGemini.numChild = parts.length >= 6 ? parseInt(parts[5]) : 0;
                requestGemini.isProvidedNumOfPassenger = true;
            }

            if (split.includes('Số lượng em bé')) {
                requestGemini.numInfant = parts.length >= 6 ? parseInt(parts[5]) : 0;
                requestGemini.isProvidedNumOfPassenger = true;
            }

            if (split.includes('Thông tin chuyến bay quý khách đã chọn')) {
                requestGemini.isSelectedFlight = true;
            }

            if (split.includes('Thông tin hạng ghế quý khách đã chọn')) {
                requestGemini.isSelectedClassSeat = true;
                requestGemini.prompt = 'hiển thị danh sách các chuyến bay bạn đã thu thập trong hệ thống không cần nhắc lại tìm kiếm theo điều kiện của người dùng và hỏi xem người dùng muốn chọn chuyến bay nào?';
                await fetch(`http://${host}:5077/api/gemini/ask`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" }
                    , body: JSON.stringify(requestGemini),
                    credentials: 'include'
                });
            }

            if (split.includes('Hạng ghế không tồn tại trong hệ thống')) {
                requestGemini.isSelectedClassSeat = false;
                requestGemini.prompt = '';
                await fetch(`http://${host}:5077/api/gemini/ask`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" }
                    , body: JSON.stringify(requestGemini),
                    credentials: 'include'
                });
            }

            if (split.includes('Bạn đã đặt chuyến bay này') && !isTriggeredBooking) {
                isTriggeredBooking = true;

                const jsonData = {
                    flight: {
                        flightId: requestGemini.selectedFlightId,
                        flightCode: requestGemini.selectedFlightCode,
                        fromCode: requestGemini.flightFromCode,
                        toCode: requestGemini.flightToCode,
                        departureTime: requestGemini.flightDepartureTime,
                        arrivalTime: requestGemini.flightArrivalTime,
                        basePrice: requestGemini.selectedFlightBasePrice,
                        manufacture: requestGemini.flightManufacture,
                        planeCode: requestGemini.flightPlaneCode,
                        tax: requestGemini.tax
                    },
                    seatPrice: requestGemini.selectedFlightSeatPrice,
                    classSeatId: requestGemini.selectedFlightSeat,
                    preorderFlights: [
                        {
                            name: 'Adult',
                            quantity: requestGemini.numAdult,
                            totalPrice: ((requestGemini.flightPrice * requestGemini.numAdult) + (requestGemini.flightPrice * requestGemini.numAdult * requestGemini.tax) + (requestGemini.selectedFlightSeatPrice * requestGemini.numAdult)),
                            seatPrice: requestGemini.selectedFlightSeatPrice,
                            tax: requestGemini.tax,
                            totalFlightPrice: requestGemini.flightPrice
                        },
                        {
                            name: 'Child',
                            quantity: requestGemini.numChild,
                            totalPrice: ((requestGemini.flightPrice * requestGemini.numChild * 0.5) + (requestGemini.flightPrice * requestGemini.numChild * requestGemini.tax * 0.5) + (requestGemini.selectedFlightSeatPrice * requestGemini.numChild * 0.5)),
                            seatPrice: requestGemini.selectedFlightSeatPrice,
                            tax: requestGemini.tax,
                            totalFlightPrice: requestGemini.flightPrice * 0.5
                        },
                        {
                            name: 'Baby',
                            quantity: requestGemini.numInfant,
                            totalPrice: (requestGemini.flightPrice * requestGemini.numInfant * (requestGemini.tax / 4)),
                            tax: requestGemini.tax,
                            totalFlightPrice: 0
                        }
                    ]
                };

                const searchFlightModel = {
                    numAdult: requestGemini.numAdult,
                    numChild: requestGemini.numChild,
                    numInfant: requestGemini.numInfant,
                }

                await bookFlightWithGemini(jsonData, searchFlightModel);
            }

            if (split.includes('Bạn đã hủy đặt chuyến bay này')) {
                requestGemini.isSelectedFlight = false;
                requestGemini.isSelectedClassSeat = false;
                requestGemini.isBookedFlight = true;
            }

            if (requestGemini.isSelectedFlight && split.includes('Id chuyến bay')) {
                requestGemini.selectedFlightId = parts.length >= 5 ? parseInt(parts[4]) : 0;
                if (requestGemini.selectedFlightId > 0) {
                    await getClassSeats();
                }
            }

            if (requestGemini.isSelectedFlight && split.includes('Từ')) {
                if (parts.length >= 5) {
                    requestGemini.flightFromCode = parts[2];
                    requestGemini.flightToCode = parts[4];
                }
            }

            if (requestGemini.isSelectedFlight && split.includes('Thời gian')) {
                if (parts.length >= 6) {
                    requestGemini.flightDepartureTime = parts[3];
                    requestGemini.flightArrivalTime = parts[5];
                }
            }

            if (requestGemini.isSelectedFlight && split.includes('Mã máy bay')) {
                requestGemini.flightPlaneCode = parts.length >= 5 ? parts[4] : '';
            }

            if (requestGemini.isSelectedFlight && split.includes('Nhà sản xuất')) {
                requestGemini.flightManufacture = parts.length >= 5 ? parts[4] : '';
            }

            if (requestGemini.isSelectedFlight && split.includes('Phí vận chuyển')) {
                requestGemini.tax = parts.length >= 5 ? parseFloat(parts[4]) : 0;
            }

            if (requestGemini.isSelectedFlight && split.includes('Giá cả')) {
                requestGemini.flightPrice = parts.length >= 4 ? parseFloat(parts[3]) : 0;
            }

            if (requestGemini.isSelectedClassSeat && split.includes('Id hạng ghế')) {
                requestGemini.selectedFlightSeat = parts.length >= 5 ? parseInt(parts[4]) : 0;
            }

            if (requestGemini.isSelectedClassSeat && split.includes('Giá hạng ghế')) {
                requestGemini.selectedFlightSeatPrice = parts.length >= 5 ? parseFloat(parts[4]) : 0;
            }

            if (split.includes('Điểm đi')) {
                filterFormWithGemini.from = parts.length >= 4 ? parts[3] : '';
            }

            if (split.includes('Điểm đến')) {
                filterFormWithGemini.to = parts.length >= 4 ? parts[3] : '';
            }

            if (split.includes('Ngày diễn ra chuyến bay')) {
                filterFormWithGemini.departureDate = parts.length >= 7 ? parts[6] : '';
            }

            if (split.includes('Chúng tôi sẽ tìm kiếm chuyến bay')) {
                await getFlightList();
                requestGemini.isSearchFlight = true;
            }
        }
    }
}

async function getFlightList() {
    const res = await fetch(`http://${host}:5077/api/gemini/searchFlight`, {
        method: "POST",
        headers: { "Content-Type": "application/json" }
        , body: JSON.stringify(filterFormWithGemini),
        credentials: 'include'
    });
    if (res.ok) {
        const result = await res.json();
        if (result?.length > 0) {
            requestGemini.prompt = 'hiển thị danh sách các chuyến bay bạn đã thu thập trong hệ thống không cần nhắc lại tìm kiếm theo điều kiện của người dùng và hỏi xem người dùng muốn chọn chuyến bay nào?';
            const res1 = await fetch(`http://${host}:5077/api/gemini/ask`, {
                method: "POST",
                headers: { "Content-Type": "application/json" }
                , body: JSON.stringify(requestGemini),
                credentials: 'include'
            });
            if (res1.ok) {
                const aiText = await res1.text();
                addBotMessage(formatFlightTextToHTML(aiText));
            }
        } else {
            addBotMessage('Chúng tôi không tìm thấy chuyến bay phù hợp với điều kiện này. Vui lòng cung cấp lại điều kiện của bạn.');
        }
    }
}

async function getClassSeats() {
    const res = await fetch(`http://${host}:5077/api/gemini/getFlightSeats?flightId=${requestGemini.selectedFlightId}`, {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: 'include'
    });
    if (res.ok) {
        requestGemini.prompt = 'hiển thị danh sách các hạng ghế của chuyến bay theo id chuyến bay được chọn bạn đã thu thập trong hệ thống';
        const res1 = await fetch(`http://${host}:5077/api/gemini/ask`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
            , body: JSON.stringify(requestGemini),
            credentials: 'include'
        });
        if (res1.ok) {
            const aiText = await res1.text();
            addBotMessage(formatFlightTextToHTML(aiText));
        }
    }
}

async function bookFlightWithGemini(flightCheckoutRequestDTO, searchFlightModel) {
    let formData = new FormData();
    formData.append('flightCheckoutRequestDTO', JSON.stringify(flightCheckoutRequestDTO));
    formData.append('flightFormDTO', JSON.stringify(searchFlightModel));

    const res = await fetch(`http://${host}:5077/api/Flight/redirect`, {
        method: 'POST',
        body: formData
    });

    if (res.ok) {
        localStorage.setItem('flightCheckoutToken', await res.json().then(res => res.token));
        addBotMessage('<text>Đặt chuyến bay thành công. Bạn có thể bấm vào </text><a href="/flight/checkout/service">đây</a><text> để hoàn tất thủ tục</text>');
    } else {
        requestGemini.isBookedFlight = false;
        const result = await res.json();
        const message = result.message || 'lý do không xác định';
        requestGemini.prompt = 'Bạn có thể nói câu: Đặt chuyến bay thất bại do ' +
            message +
            ' và yêu cầu người dùng tìm kiếm lại chuyến bay theo các tiêu chí như ban đầu ở trên.'
        const res1 = await fetch(`http://${host}:5077/api/gemini/ask`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
            , body: JSON.stringify(requestGemini),
            credentials: 'include'
        });
        if (res1.ok) {
            const aiText = await res1.text();
            addBotMessage(formatFlightTextToHTML(aiText));
        }
        requestGemini.isProvidedNumOfPassenger = false;
        requestGemini.isSelectedFlight = false;
        requestGemini.isSelectedClassSeat = false;
        requestGemini.isBookedFlight = true;
        requestGemini.isSearchFlight = true;
    }
}


function formatFlightTextToHTML(aiText) {
    let html = aiText.replace(/\n/g, "<br>");
    return html;
}
