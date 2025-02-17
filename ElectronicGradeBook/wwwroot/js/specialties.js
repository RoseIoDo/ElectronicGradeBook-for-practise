$(document).ready(function () {
    console.log("specialties.js loaded");

    loadSpecialties();

    // Обробник для кнопки "Додати спеціальність"
    $("#addSpecialtyBtn").click(function () {
        clearForm();
        $("#specialtyModalLabel").text("Додати спеціальність");
        // Завантаження даних для випадаючих списків
        loadFacultyOptions();
        loadProgramOptions();
        $("#specialtyModal").modal("show");
    });

    // Обробник для кнопки редагування
    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#specialtyModalLabel").text("Редагувати спеціальність");

        $("#specId").val($(this).data("id"));
        $("#specName").val($(this).data("name"));
        $("#shortName").val($(this).data("short"));
        $("#code").val($(this).data("code"));
        // Завантаження даних для селектів із встановленням вибраного значення
        loadFacultyOptions($(this).data("facultyid"));
        loadProgramOptions($(this).data("programid"));

        $("#specialtyModal").modal("show");
    });

    // Збереження спеціальності (додавання/редагування)
    $("#saveSpecialtyBtn").click(function () {
        const idVal = parseInt($("#specId").val()) || 0;
        const nameVal = $("#specName").val().trim();
        const shortVal = $("#shortName").val().trim();
        const codeVal = parseInt($("#code").val()) || 0;
        const facVal = parseInt($("#facultySelect").val()) || 0;
        const progVal = parseInt($("#programSelect").val()) || 0;

        if (!nameVal || !shortVal) {
            alert("Назва і ShortName не можуть бути пустими.");
            return;
        }

        const url = idVal ? "/Specialties/Edit" : "/Specialties/Add";
        const dataObj = {
            id: idVal,
            name: nameVal,
            shortName: shortVal,
            code: codeVal,
            facultyId: facVal,
            programId: progVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#specialtyModal").modal("hide");
                    loadSpecialties();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження спеціальності.");
            }
        });
    });

    // Видалення спеціальності
    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити спеціальність?")) {
            $.ajax({
                url: `/Specialties/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadSpecialties();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка при видаленні.");
                }
            });
        }
    });

    // Функція завантаження таблиці спеціальностей
    function loadSpecialties() {
        $.ajax({
            url: "/Specialties/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#specialtiesTable tbody");
                $tbody.empty();
                list.forEach(s => {
                    $tbody.append(`
                        <tr>
                            <td>${s.id}</td>
                            <td>${s.name}</td>
                            <td>${s.shortName}</td>
                            <td>${s.code}</td>
                            <td>${s.facultyName || ""}</td>
                            <td>${s.programName || ""}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${s.id}"
                                    data-name="${s.name}"
                                    data-short="${s.shortName}"
                                    data-code="${s.code}"
                                    data-facultyid="${s.facultyId}"
                                    data-programid="${s.programId}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${s.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження спеціальностей.");
            }
        });
    }

    // Функція очищення форми
    function clearForm() {
        $("#specId").val("");
        $("#specName").val("");
        $("#shortName").val("");
        $("#code").val("");
        $("#facultySelect").empty();
        $("#programSelect").empty();
    }

    // Функція завантаження факультетів у випадаючий список
    function loadFacultyOptions(selectedId) {
        $.ajax({
            url: "/Faculties/GetAll",
            type: "GET",
            success: function (data) {
                console.log("Faculties data:", data);
                var $facultySelect = $("#facultySelect");
                $facultySelect.empty();
                $facultySelect.append('<option value="">Оберіть факультет</option>');
                $.each(data, function (i, faculty) {
                    // Оскільки JSON має властивості "id" і "name", використовуємо їх
                    $facultySelect.append('<option value="' + faculty.id + '">' + faculty.name + '</option>');
                });
                if (selectedId) {
                    $facultySelect.val(selectedId);
                }
            },
            error: function () {
                alert("Помилка завантаження факультетів.");
            }
        });
    }

    // Функція завантаження освітніх програм у випадаючий список
    function loadProgramOptions(selectedId) {
        $.ajax({
            url: "/StudyPrograms/GetAll",
            type: "GET",
            success: function (data) {
                console.log("Programs data:", data);
                var $programSelect = $("#programSelect");
                $programSelect.empty();
                $programSelect.append('<option value="">Оберіть програму</option>');
                $.each(data, function (i, program) {
                    $programSelect.append('<option value="' + program.id + '">' + program.name + '</option>');
                });
                if (selectedId) {
                    $programSelect.val(selectedId);
                }
            },
            error: function () {
                alert("Помилка завантаження освітніх програм.");
            }
        });
    }
});
