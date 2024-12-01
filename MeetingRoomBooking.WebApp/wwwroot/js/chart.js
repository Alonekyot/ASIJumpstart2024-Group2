async function loadChartData() {
    const response = await fetch('/Home/GetChartData');
    const data = await response.json();

    const ctx = document.getElementById('myChart').getContext('2d');
    new Chart(ctx, {
        type: 'bar', 
        data: data,
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

loadChartData();    