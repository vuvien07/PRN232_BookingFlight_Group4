async function SubmitFlightInfo(e) {
    e.preventDefault();
    const labels = ['DepartureDate', 'NumAdult', 'NumChild', 'NumInfant']
    try {
        const res = await fetch(`http://${host}:5077/api/Home`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "From": document.querySelector('select[name="From"]').value,
                "To": document.querySelector('select[name="To"]').value,
                "DepartureDate": document.querySelector('input[name="DepartureDate"]').value,
                "NumAdult": parseInt(document.querySelector('input[name="NumAdult"]').value),
                "NumChild": parseInt(document.querySelector('input[name="NumChild"]').value),
                "NumInfant": parseInt(document.querySelector('input[name="NumInfant"]').value)
            })
        });
        if (!res.ok) {
            const result = await res.json();
            if (result) {
                let errors = result;
                for (let i = 0; i < labels.length; i++) {
                    DisplayError(labels[i], errors);
                }
            }
            if (result.message) {
                await showSnackbar(result.message, "error");
            }
        } else {
            let json = await res.json();
            localStorage.setItem("flightInfoToken", json.token);
            window.location.href = '/flight/list';
        }
    } catch(error) {
        showSnackbar("Error", "error");
    }
}

function DisplayError(id, errors) {
    if (!errors || !errors[id]) {
        document.getElementById(id).innerText = '';
        return;
    };
    document.getElementById(id).innerText = errors[id];
}