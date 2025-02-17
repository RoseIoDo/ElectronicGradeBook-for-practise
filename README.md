# 🎓 Electronic Grade Book

📌 **У майбутньому планується реалізація наступних покращень та нових функцій:**

---

## 🔍 Глобальні зміни
✔ Додати пошук на всіх сторінках, включаючи фільтри.  
✔ Реалізувати фільтрацію та пагінацію для всіх сторінок уніфікованим способом.  
✔ Автоматичне оновлення всіх даних при зміні академічного року.  
✔ Прив’язка інформації до конкретних дат для точнішого відображення даних.  
✔ Впровадити архівування записів, щоб розділити актуальні та застарілі дані.  
✔ Додати можливість масового видалення записів.

---

## 🗄 База даних
✔ Додати до інститутів поле **скороченої назви**.  
✔ Додати до спеціальностей поле **ООП (освітня програма)**.  

---

## 🎨 UI / Логіка

### 📂 **Загальні зміни для всіх сторінок**
✔ Використовувати скорочені назви факультетів у таблицях.

### 🏛 **Сторінка факультетів**
✔ Додати скорочену назву для інститутів.

### 🎓 **Сторінка спеціальностей**
✔ Створити вкладки для факультетів і освітніх програм.  
✔ Поле **коду спеціальності** змінити з `int` на `string`, оскільки код може містити символи (наприклад, `F3`).  
✔ Додати можливість дублювати коди спеціальностей та перевірку унікальності за скороченою назвою та освітньою програмою.  
✔ **Опційно**: Додати поле **ООП** у БД.

### 🏫 **Сторінка груп**
✔ Винести вибір дати в окрему кнопку у верхній частині сторінки.  
✔ Додати дві вкладки: **Актуальні групи** та **Випущені групи**.  
✔ Реалізувати вкладки за спеціальностями.  
✔ Автоматичне визначення **академічного року** та **номеру групи** за датами вступу та випуску.

### 👩‍🎓 **Сторінка студентів**
✔ Додати поле для **причини неактивності** студента (наприклад, академвідпустка).  
✔ Реалізувати **пагінацію** та можливість згорнути список груп.  
✔ Додати позначку **індивідуальний план** для студентів.  
✔ Перемістити кнопку **"Додати студента"** над списком.  
✔ Додати можливість додавати **декілька студентів одночасно**.  
✔ Додати дві вкладки: **Актуальні студенти** та **Випущені студенти**.

### 👨‍🏫 **Сторінка викладачів**
✔ Перемістити кнопку **"Додати викладача"** над таблицею.  
✔ Додати можливість **прив’язки викладача до кафедри чи факультету**.

### 📚 **Сторінка предметів**
✔ Реалізувати вкладки: **Основні предмети** та **Вибіркові предмети** (поділ вибіркових за циклами).  
✔ Вибір циклу доступний **тільки при виборі вибіркового предмета** (за замовчуванням предмет основний).  
✔ Додати розподіл предметів за **факультетами та спеціальностями**.

### 🏫 **Сторінка викладання предметів**
✔ Реалізувати можливість **вибору різних викладачів або кількох викладачів** для однієї групи (наприклад, для підгруп).

### 🔄 **Сторінка підгруп**
✔ При натисканні на підгрупу виводити **повний список студентів**.  
✔ Реалізувати **модальні вікна** для додавання та видалення підгруп.

---

## 📊 **Сторінка GradeBook**
✔ Після вибору групи та натискання "Завантажити" спочатку мають з’явитися **роки**, а у них **вкладки з семестрами** (роки залежать від року вступу та випуску).  
✔ Після вибору курсу має з’являтися випадаючий список підгруп (якщо вони є).  
✔ Додати розрахунок **середнього балу** за формулою та оцінки за активність.  
✔ Реалізувати **генерацію таблиці з оцінками** зі всіх предметів для кожного студента.  
✔ Змінити виведення оцінок: **виводити не по групі, а по студенту**, враховуючи всі групи та підгрупи, до яких він належить (включаючи вибіркові дисципліни).  

---

## 🚀 **Плани на майбутнє**
- Оптимізація продуктивності.  
- Покращення мобільної версії.  
- Додавання нових аналітичних функцій.  
- Інтеграція з зовнішніми сервісами.

📌 **Проєкт у процесі активної розробки. Stay tuned!** 🎉
