const baseURL = "http://localhost:5173/api"; // Backend API URL'si

const cities = [
    "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin", "Aydın",
    "Balıkesir", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale", "Çankırı",
    "Çorum", "Denizli", "Diyarbakır", "Edirne", "Elazığ", "Erzincan", "Erzurum", "Eskişehir",
    "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Isparta", "Mersin", "İstanbul",
    "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir", "Kocaeli", "Konya",
    "Kütahya", "Malatya", "Manisa", "Kahramanmaraş", "Mardin", "Muğla", "Muş", "Nevşehir",
    "Niğde", "Ordu", "Rize", "Sakarya", "Samsun", "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat",
    "Trabzon", "Tunceli", "Şanlıurfa", "Uşak", "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt",
    "Karaman", "Kırıkkale", "Batman", "Şırnak", "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük",
    "Kilis", "Osmaniye", "Düzce"
];

// Şehir seçeneklerini yükle
const citySelect = document.getElementById('city');
cities.forEach(city => {
    const option = document.createElement('option');
    option.value = city;
    option.textContent = city;
    citySelect.appendChild(option);
});

// To-do item ekleme butonuna tıklanınca
document.getElementById('add-btn').addEventListener('click', async function () {
    const city = document.getElementById('city').value;
    const date = document.getElementById('date').value;

    if (city && date) {
        const list = document.getElementById('places-list');
        const listItem = document.createElement('li');

        const text = document.createElement('span');
        text.textContent = `${city} - ${date}`;

        const checkbox = document.createElement('input');
        checkbox.type = 'checkbox';
        checkbox.addEventListener('change', function () {
            if (checkbox.checked && !listItem.querySelector('button')) {
                const commentButton = document.createElement('button');
                commentButton.textContent = 'Yorum ve Fotoğraf Ekle';
                commentButton.addEventListener('click', function () {
                    window.open('feature_page.html', '_blank');
                });
                listItem.appendChild(commentButton);
            } else if (!checkbox.checked) {
                const existingButton = listItem.querySelector('button');
                if (existingButton) {
                    existingButton.remove();
                }
            }
        });

        listItem.appendChild(text);
        listItem.appendChild(checkbox);
        list.appendChild(listItem);

        // Backend'e yeni to-do item'ı gönder
        try {
            const token = localStorage.getItem("jwt"); // JWT token'ı al

            // Eğer token bulunamazsa hata mesajı göster
            if (!token) {
                alert('Giriş yapmanız gerekmektedir.');
                return;
            }

            const response = await fetch(`${baseURL}/todo/add`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}` // JWT'yi header'a ekle
                },
                body: JSON.stringify({
                    city: city,
                    travelDate: date
                })
            });

            // HTTP hata kodu kontrolü
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Hata: ${response.status} - ${errorText}`);
            }

            // Başarılı olduğunda
            const data = await response.json();
            console.log("Yeni Todo başarıyla eklendi:", data);
            alert('Yeni gezilecek yer başarıyla eklendi.');
        } catch (err) {
            // Herhangi bir hata oluştuğunda bu bloğa düşer
            console.error("Veri gönderme hatası:", err);

            // Hata mesajını kullanıcıya göster
            if (err.message.includes("NetworkError")) {
                alert("Sunucuya bağlanırken bir hata oluştu. Lütfen internet bağlantınızı kontrol edin.");
            } else {
                alert(`Veri gönderilirken bir hata oluştu: ${err.message}`);
            }
        }
    } else {
        alert('Lütfen bir şehir ve tarih seçiniz.');
    }
});
