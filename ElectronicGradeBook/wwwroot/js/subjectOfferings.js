$(document).ready(function () {
    console.log("subjectOfferings.js loaded");

    initYearAndSemesterSelects();
    loadOfferings();
    loadDropdownData();

    // НАТИСНУТИ "Додати"
    $("#addSoBtn").click(function () {
        clearForm();
        $("#soModalLabel").text("Додати предмет-викладання");
        $("#soModal").modal("show");
    });

    // Редагування
    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#soModalLabel").text("Редагувати предмет-викладання");

        // Заповнюємо поля ID, subjectId, teacherId тощо
        $("#soId").val($(this).data("id"));
        $("#subjectSelect").val($(this).data("subjectid"));
        $("#teacherSelect").val($(this).data("teacherid"));
        $("#yearOfStudy").val($(this).data("year"));
        $("#semesterInYear").val($(this).data("sem"));
        $("#credits").val($(this).data("credits"));

        // Якщо цей Offering має кілька груп, ми не можемо відобразити всі (випадаючий список один).
        // Тому для спрощення покажемо “(не змінюємо)”.
        // Якщо потрібно, можете довантажити точний зв’язок і встановити groupSelect.
        $("#groupSelect").html(`
            <option value="">(не змінюємо)</option>
        `);

        $("#soModal").modal("show");
    });

    // Збереження (Add / Edit)
    $("#saveSoBtn").click(function () {
        const idVal = parseInt($("#soId").val()) || 0;
        const subjVal = parseInt($("#subjectSelect").val()) || 0;
        const teachVal = parseInt($("#teacherSelect").val()) || 0;
        const groupSel = $("#groupSelect").val() || "";
        const yearVal = parseInt($("#yearOfStudy").val()) || 1;
        const semVal = parseInt($("#semesterInYear").val()) || 1;
        const credVal = parseFloat($("#credits").val()) || 0;

        if (!subjVal) {
            alert("Оберіть предмет!");
            return;
        }
        if (!teachVal) {
            alert("Оберіть викладача!");
            return;
        }

        // Формуємо об'єкт для надсилання
        let dataObj = {
            id: idVal,
            subjectId: subjVal,
            teacherId: teachVal,
            yearOfStudy: yearVal,
            semesterInYear: semVal,
            credits: credVal
        };

        const url = idVal ? "/SubjectOfferings/Edit" : "/SubjectOfferings/Add";

        if (!idVal) {
            // Це РЕЖИМ "Add"
            // Якщо обрано “course-…”
            if (groupSel.startsWith("course-")) {
                dataObj.groupNameOrCourse = groupSel;  // напр. "course-KN-1"
                dataObj.singleGroupId = null;
            }
            else if (groupSel) {
                // Одна конкретна група
                const gId = parseInt(groupSel);
                if (gId > 0) {
                    dataObj.singleGroupId = gId;
                    dataObj.groupNameOrCourse = "";
                } else {
                    // Якщо користувач повернув "", значить “(Без групи / Вибірковий)”
                    dataObj.singleGroupId = null;
                    dataObj.groupNameOrCourse = "";
                }
            }
            else {
                // Якщо взагалі пусто
                dataObj.singleGroupId = null;
                dataObj.groupNameOrCourse = "";
            }
        }
        else {
            // РЕЖИМ "Edit": для спрощення ми не змінюємо зв’язки з групами,
            // тому нічого не робимо з groupSel. 
            // (Якщо треба, реалізуйте тут.)
        }

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#soModal").modal("hide");
                    loadOfferings();
                } else {
                    alert("Помилка: " + resp.message);
                }
            },
            error: function () {
                alert("Помилка запиту (Add/Edit).");
            }
        });
    });

    // Видалення
    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити запис?")) {
            $.ajax({
                url: `/SubjectOfferings/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        loadOfferings();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення SubjectOffering.");
                }
            });
        }
    });

    // ======================== ФУНКЦІЇ ========================

    function initYearAndSemesterSelects() {
        const $yr = $("#yearOfStudy");
        const $sem = $("#semesterInYear");
        $yr.empty();
        $sem.empty();
        for (let y = 1; y <= 4; y++) {
            $yr.append(`<option value="${y}">${y}</option>`);
        }
        for (let s = 1; s <= 2; s++) {
            $sem.append(`<option value="${s}">${s}</option>`);
        }
    }

    // Завантажуємо список SubjectOfferings і показуємо в таблиці
    function loadOfferings() {
        $.ajax({
            url: "/SubjectOfferings/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#soTable tbody");
                $tbody.empty();
                if (!list || list.length === 0) {
                    $tbody.append("<tr><td colspan='8' class='text-center'>Немає записів</td></tr>");
                    return;
                }
                list.forEach(o => {
                    // o.combinedGroupNames => масив string[]
                    let grpLabel = "—";
                    if (o.combinedGroupNames && o.combinedGroupNames.length > 0) {
                        grpLabel = o.combinedGroupNames.join(", ");
                    }
                    $tbody.append(`
                        <tr>
                            <td>${o.id}</td>
                            <td>${o.subjectName}</td>
                            <td>${o.teacherName}</td>
                            <td>${grpLabel}</td>
                            <td>${o.yearOfStudy}</td>
                            <td>${o.semesterInYear}</td>
                            <td>${o.credits}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${o.id}"
                                    data-subjectid="${o.subjectId}"
                                    data-teacherid="${o.teacherId}"
                                    data-year="${o.yearOfStudy}"
                                    data-sem="${o.semesterInYear}"
                                    data-credits="${o.credits}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${o.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження SubjectOfferings.");
            }
        });
    }

    // Паралельно завантажимо предмети, викладачі, групи
    function loadDropdownData() {
        loadSubjects();
        loadTeachers();
        loadGroupsForDropdown();
    }

    // ============== Кожен із цих 3 методів викликаємо один раз у loadDropdownData() =============

    function loadSubjects() {
        $.ajax({
            url: "/Subjects/GetAll",
            type: "GET",
            success: function (data) {
                const $sel = $("#subjectSelect");
                $sel.empty();
                $sel.append(`<option value="">-- Предмет --</option>`);
                if (data && data.length > 0) {
                    data.forEach(s => {
                        $sel.append(`<option value="${s.id}">${s.fullName}</option>`);
                    });
                }
            }
        });
    }

    function loadTeachers() {
        $.ajax({
            url: "/Teachers/GetAll",
            type: "GET",
            success: function (data) {
                const $sel = $("#teacherSelect");
                $sel.empty();
                $sel.append(`<option value="">-- Викладач --</option>`);
                if (data && data.length > 0) {
                    data.forEach(t => {
                        $sel.append(`<option value="${t.id}">${t.fullName}</option>`);
                    });
                }
            }
        });
    }

    // Завантаження груп і формування опцій (course-…)
    function loadGroupsForDropdown() {
        $.ajax({
            url: "/Groups/GetAll",
            type: "GET",
            success: function (list) {
                const $sel = $("#groupSelect");
                $sel.empty();
                $sel.append(`<option value="">(Без групи / Вибірковий)</option>`);

                if (!list || list.length === 0) {
                    $sel.append("<option disabled>Немає груп</option>");
                    return;
                }

                // Згрупуємо: key = "КН-1", value = масив
                const map = {};
                list.forEach(g => {
                    const key = g.groupPrefix + "-" + g.currentStudyYear;
                    if (!map[key]) map[key] = [];
                    map[key].push(g);
                });

                Object.keys(map).forEach(key => {
                    // Спершу – опція “course-KN-1”
                    $sel.append(`<option value="course-${key}">${key} (весь курс)</option>`);
                    // Потім кожна група
                    map[key].forEach(g => {
                        const label = `${g.groupPrefix}-${g.groupNumber} (ID=${g.id})`;
                        $sel.append(`<option value="${g.id}">   ${label}</option>`);
                    });
                });
            }
        });
    }

    // Очищення форми (перед "Додати"/"Редагувати")
    function clearForm() {
        $("#soId").val("");
        $("#subjectSelect").val("");
        $("#teacherSelect").val("");
        $("#groupSelect").val("");
        $("#yearOfStudy").val("1");
        $("#semesterInYear").val("1");
        $("#credits").val("0");
    }
});
