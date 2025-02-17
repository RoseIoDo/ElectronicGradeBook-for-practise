$(document).ready(function () {
    console.log("subjectSubgroups.js loaded");

    // Ініціалізація: завантаження предметних пропозицій та викладачів для підгруп
    loadOfferingsRegular();  // Завантажити курсові предмети
    loadOfferingsElective(); // Завантажити вибіркові предмети
    loadTeachers();          // Завантажити викладачів для підгруп

    // --- Події для вкладки "Курсові" ---
    $("#loadSubgroupsBtn").click(function () {
        const offId = parseInt($("#offeringSelect").val()) || 0;
        if (!offId) {
            alert("Оберіть предмет-викладання (курсовий).");
            return;
        }
        loadSubgroups(offId, false); // false => курсовий
    });

    $("#addSubgroupBtn").click(function () {
        const offId = parseInt($("#offeringSelect").val()) || 0;
        if (!offId) {
            alert("Оберіть предмет-викладання!");
            return;
        }
        clearSubgroupForm();
        $("#sgOfferingId").val(offId);
        $("#subgroupModalLabel").text("Додати підгрупу (курсовий)");
        $("#subgroupModal").modal("show");
    });

    // --- Події для вкладки "Вибіркові" ---
    $("#loadElectiveSubgroupsBtn").click(function () {
        const offId = parseInt($("#electiveSelect").val()) || 0;
        if (!offId) {
            alert("Оберіть вибірковий предмет!");
            return;
        }
        loadSubgroups(offId, true); // true => вибірковий
    });

    $("#addElectiveSubgroupBtn").click(function () {
        const offId = parseInt($("#electiveSelect").val()) || 0;
        if (!offId) {
            alert("Оберіть вибірковий предмет!");
            return;
        }
        clearSubgroupForm();
        $("#sgOfferingId").val(offId);
        $("#subgroupModalLabel").text("Додати підгрупу (вибірковий)");
        $("#subgroupModal").modal("show");
    });

    // --- Збереження підгрупи (Add/Edit) ---
    $("#saveSubgroupBtn").click(function () {
        const idVal = parseInt($("#sgId").val()) || 0;
        const offVal = parseInt($("#sgOfferingId").val()) || 0;
        const nameVal = $("#sgName").val().trim();
        const teacherVal = parseInt($("#sgTeacherSelect").val()) || null;

        if (!offVal) {
            alert("Немає SubjectOffering!");
            return;
        }
        if (!nameVal) {
            alert("Назва підгрупи пуста.");
            return;
        }

        const url = idVal ? "/SubjectSubgroups/Edit" : "/SubjectSubgroups/Add";
        const dataObj = {
            id: idVal,
            subjectOfferingId: offVal,
            name: nameVal,
            teacherId: teacherVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#subgroupModal").modal("hide");
                    reloadSubgroupsAfterSave(offVal);
                } else {
                    alert("Помилка: " + resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження підгрупи.");
            }
        });
    });

    // --- Редагування підгрупи ---
    $(document).on("click", ".edit-subgroup-btn", function () {
        clearSubgroupForm();
        $("#subgroupModalLabel").text("Редагувати підгрупу");

        $("#sgId").val($(this).data("id"));
        $("#sgOfferingId").val($(this).data("offeringid"));
        $("#sgName").val($(this).data("name"));
        $("#sgTeacherSelect").val($(this).data("teacherid") || "");

        $("#subgroupModal").modal("show");
    });

    // --- Видалення підгрупи ---
    $(document).on("click", ".delete-subgroup-btn", function () {
        const id = $(this).data("id");
        if (!confirm("Видалити підгрупу?")) return;
        $.ajax({
            url: `/SubjectSubgroups/Delete?id=${id}`,
            type: "POST",
            success: function (resp) {
                if (resp.success) {
                    alert(resp.message);
                    const off = parseInt($("#sgOfferingId").val()) || 0;
                    reloadSubgroupsAfterSave(off);
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка видалення підгрупи.");
            }
        });
    });

    // --- Додавання студента в підгрупу (спрощено: введення ID студента) ---
    $(document).on("click", ".add-student-btn", function () {
        const sgId = $(this).data("subgroup");
        const stId = prompt("Введіть ID студента для додавання:");
        if (!stId) return;
        $.ajax({
            url: `/SubjectSubgroups/AddStudent?subgroupId=${sgId}&studentId=${stId}`,
            type: "POST",
            success: function (resp) {
                if (!resp.success) {
                    alert(resp.message);
                } else {
                    alert("Студента додано до підгрупи.");
                }
            },
            error: function () {
                alert("Помилка додавання студента.");
            }
        });
    });

    // --- Видалення студента з підгрупи ---
    $(document).on("click", ".remove-student-btn", function () {
        const sgId = $(this).data("subgroup");
        const stId = prompt("Введіть ID студента для вилучення:");
        if (!stId) return;
        $.ajax({
            url: `/SubjectSubgroups/RemoveStudent?subgroupId=${sgId}&studentId=${stId}`,
            type: "POST",
            success: function (resp) {
                if (!resp.success) {
                    alert(resp.message);
                } else {
                    alert("Студента вилучено з підгрупи.");
                }
            },
            error: function () {
                alert("Помилка вилучення студента.");
            }
        });
    });

    // --- Функції для основної сторінки та перезавантаження даних ---
    function reloadSubgroupsAfterSave(offId) {
        if (!offId) return;
        const regOff = parseInt($("#offeringSelect").val());
        const elOff = parseInt($("#electiveSelect").val());
        if (offId === regOff) {
            loadSubgroups(offId, false);
        } else if (offId === elOff) {
            loadSubgroups(offId, true);
        } else {
            loadSubgroups(offId, false);
        }
    }

    function loadOfferingsRegular() {
        $.ajax({
            url: "/SubjectOfferings/GetAll",
            type: "GET",
            success: function (list) {
                const $sel = $("#offeringSelect");
                $sel.empty();
                $sel.append(`<option value="">-- курсовий предмет --</option>`);
                if (list && list.length > 0) {
                    list.forEach(o => {
                        if (!o.isSubjectElective) {
                            let gLabel = (o.combinedGroupNames && o.combinedGroupNames.length > 0)
                                ? o.combinedGroupNames.join(", ")
                                : "(немає)";
                            let label = `${o.subjectName} [${gLabel}] (sem=${o.semesterInYear})`;
                            $sel.append(`<option value="${o.id}">${label}</option>`);
                        }
                    });
                }
            },
            error: function () {
                alert("Помилка завантаження курсових предметів.");
            }
        });
    }

    function loadOfferingsElective() {
        $.ajax({
            url: "/SubjectOfferings/GetAll",
            type: "GET",
            success: function (list) {
                const $sel = $("#electiveSelect");
                $sel.empty();
                $sel.append(`<option value="">-- вибірковий предмет --</option>`);
                if (list && list.length > 0) {
                    list.forEach(o => {
                        if (o.isSubjectElective) {
                            let gLabel = (o.combinedGroupNames && o.combinedGroupNames.length > 0)
                                ? o.combinedGroupNames.join(", ")
                                : "(вільний)";
                            let label = `${o.subjectName} [${gLabel}] (sem=${o.semesterInYear})`;
                            $sel.append(`<option value="${o.id}">${label}</option>`);
                        }
                    });
                }
            },
            error: function () {
                alert("Помилка завантаження вибіркових предметів.");
            }
        });
    }

    function loadSubgroups(offId, isElective) {
        const tableId = isElective ? "#electiveSubgroupsTable" : "#subgroupsTable";
        $.ajax({
            url: `/SubjectSubgroups/GetByOffering?offeringId=${offId}`,
            type: "GET",
            success: function (subs) {
                const $tbody = $(`${tableId} tbody`);
                $tbody.empty();
                if (!subs || subs.length === 0) {
                    $tbody.append(`<tr><td colspan="5" class="text-center">Немає підгруп</td></tr>`);
                    return;
                }
                subs.forEach(sg => {
                    $tbody.append(`
                        <tr>
                            <td>${sg.id}</td>
                            <td>${sg.name}</td>
                            <td>${sg.teacherName || ""}</td>
                            <td>
                                <button class="btn btn-info btn-sm add-student-btn" data-subgroup="${sg.id}">
                                    +Студент
                                </button>
                                <button class="btn btn-danger btn-sm remove-student-btn" data-subgroup="${sg.id}">
                                    -Студент
                                </button>
                            </td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-subgroup-btn"
                                    data-id="${sg.id}"
                                    data-offeringid="${sg.subjectOfferingId}"
                                    data-name="${sg.name}"
                                    data-teacherid="${sg.teacherId || ""}"
                                >Редагувати</button>
                                <button class="btn btn-danger btn-sm delete-subgroup-btn"
                                    data-id="${sg.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження підгруп.");
            }
        });
    }

    function loadTeachers() {
        $.ajax({
            url: "/Teachers/GetAll",
            type: "GET",
            success: function (list) {
                const $sel = $("#sgTeacherSelect");
                $sel.empty();
                $sel.append(`<option value="">-- Без викладача --</option>`);
                if (list && list.length > 0) {
                    list.forEach(t => {
                        $sel.append(`<option value="${t.id}">${t.fullName}</option>`);
                    });
                }
            },
            error: function () {
                console.warn("Помилка завантаження викладачів.");
            }
        });
    }

    // --- Функції для модального вікна "Додати студента" (поки не потрібні, якщо додаємо за ID) ---
    // Якщо в майбутньому потрібно розширити логіку пошуку за факультетом/спеціальністю – можна буде розкоментувати ці функції
    function clearAddStudentModal() {
        $("#modalFacultySelect").val("");
        $("#modalGroupSelect").empty().append(`<option value="">-- Група --</option>`);
        $("#modalStudentSelect").empty().append(`<option value="">-- Студент --</option>`);
    }

    // Очищення форми підгруп (додавання/редагування)
    function clearSubgroupForm() {
        $("#sgId").val("");
        $("#sgOfferingId").val("");
        $("#sgName").val("");
        $("#sgTeacherSelect").val("");
    }
});
