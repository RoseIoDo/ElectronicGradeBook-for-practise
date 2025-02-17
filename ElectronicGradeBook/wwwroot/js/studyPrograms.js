$(document).ready(function () {
    const currentPage = 1;
    const pageSize = 10;

    // Завантаження всіх освітніх програм
    function loadStudyPrograms() {
        $.ajax({
            url: "/StudyPrograms/GetAll",
            type: "GET",
            success: function (programs) {
                renderStudyPrograms(programs);
            },
            error: function () {
                alert("Помилка завантаження освітніх програм.");
            }
        });
    }

    // Рендеринг таблиці програм
    function renderStudyPrograms(programs) {
        const $tbody = $("#studyProgramsTable tbody");
        $tbody.empty();
        programs.forEach(function (program) {
            const row = `
                <tr>
                    <td>${program.id}</td>
                    <td>${program.name}</td>
                    <td>${program.durationYears}</td>
                    <td>
                        <button class="btn btn-warning btn-sm edit-btn"
                                data-id="${program.id}"
                                data-name="${program.name}"
                                data-dur="${program.durationYears}">
                            Редагувати
                        </button>
                        <button class="btn btn-danger btn-sm delete-btn"
                                data-id="${program.id}">
                            Видалити
                        </button>
                    </td>
                </tr>
            `;
            $tbody.append(row);
        });
    }

    // Очищення форми
    function clearForm() {
        $("#spId").val("");
        $("#spName").val("");
        $("#spDuration").val("");
    }

    // Відкриття модального вікна для додавання нової програми
    $("#addProgramBtn").on("click", function () {
        clearForm();
        $("#studyProgramModalLabel").text("Додати освітню програму");
        $("#studyProgramModal").modal("show");
    });

    // Обробка кліку на кнопку редагування
    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#studyProgramModalLabel").text("Редагувати програму");
        $("#spId").val($(this).data("id"));
        $("#spName").val($(this).data("name"));
        $("#spDuration").val($(this).data("dur"));
        $("#studyProgramModal").modal("show");
    });

    // Збереження (додавання або редагування) програми
    $("#saveProgramBtn").on("click", function () {
        const id = parseInt($("#spId").val()) || 0;
        const name = $("#spName").val().trim();
        const duration = parseInt($("#spDuration").val()) || 0;

        if (!name) {
            alert("Назва не може бути порожньою.");
            return;
        }

        const url = id ? "/StudyPrograms/Edit" : "/StudyPrograms/Add";
        const dataObj = { id: id, name: name, durationYears: duration };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (response) {
                if (response.success) {
                    $("#studyProgramModal").modal("hide");
                    loadStudyPrograms();
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Помилка сервера під час збереження програми.");
            }
        });
    });

    // Обробка видалення програми
    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити програму?")) {
            $.ajax({
                url: `/StudyPrograms/Delete?id=${id}`,
                type: "POST",
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        loadStudyPrograms();
                    } else {
                        alert(response.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення програми.");
                }
            });
        }
    });

    // Приклад функції для завантаження програм з фільтром (якщо потрібно)
    function loadFilteredPrograms() {
        const search = $("#searchProgram").val() || "";
        const durationFilter = parseInt($("#durationFilter").val()) || null;

        $.ajax({
            url: "/StudyPrograms/GetFiltered",
            type: "GET",
            data: {
                search: search,
                durationYears: durationFilter,
                pageNumber: currentPage,
                pageSize: pageSize
            },
            success: function (response) {
                if (response.success) {
                    renderStudyPrograms(response.data.items);
                    // Додатково можна опрацювати пагінацію
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Помилка завантаження програм з фільтром.");
            }
        });
    }

    // Початкове завантаження даних
    loadStudyPrograms();
});
