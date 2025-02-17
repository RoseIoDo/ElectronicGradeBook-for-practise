$(document).ready(function () {

    const $tableBody = $("#saTable tbody");
    const $saModal = $("#saModal");

    const $saId = $("#saId");
    const $studentSelect = $("#studentSelect");
    const $activitySelect = $("#activitySelect");
    const $dateAwarded = $("#dateAwarded");
    const $semester = $("#semester");
    const $notes = $("#notes");

    loadSA();
    loadDropdownData();

    $("#addSaBtn").click(function () {
        clearForm();
        $("#saModalLabel").text("Додати студентську активність");
        $saModal.modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#saModalLabel").text("Редагувати");

        $saId.val($(this).data("id"));
        $studentSelect.val($(this).data("studentid"));
        $activitySelect.val($(this).data("activityid"));
        $dateAwarded.val($(this).data("date"));
        $semester.val($(this).data("sem"));
        $notes.val($(this).data("notes"));

        $saModal.modal("show");
    });

    $("#saveSaBtn").click(function () {
        const idVal = parseInt($saId.val()) || 0;
        const stVal = parseInt($studentSelect.val()) || 0;
        const actVal = parseInt($activitySelect.val()) || 0;
        const dateVal = $dateAwarded.val() || null;
        const semVal = $semester.val().trim();
        const notesVal = $notes.val().trim();

        const url = idVal ? "/StudentActivities/Edit" : "/StudentActivities/Add";
        const dataObj = {
            id: idVal,
            studentId: stVal,
            activityId: actVal,
            dateAwarded: dateVal,
            semester: semVal,
            notes: notesVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $saModal.modal("hide");
                    loadSA();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження активності.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити запис?")) {
            $.ajax({
                url: `/StudentActivities/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadSA();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення.");
                }
            });
        }
    });

    function loadSA() {
        $.ajax({
            url: "/StudentActivities/GetAll",
            type: "GET",
            success: function (data) {
                $tableBody.empty();
                data.forEach(sa => {
                    const dateStr = sa.dateAwarded ? sa.dateAwarded.split("T")[0] : "";
                    $tableBody.append(`
                        <tr>
                            <td>${sa.id}</td>
                            <td>${sa.studentName}</td>
                            <td>${sa.activityName}</td>
                            <td>${dateStr}</td>
                            <td>${sa.semester || ""}</td>
                            <td>${sa.notes || ""}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${sa.id}"
                                    data-studentid="${sa.studentId}"
                                    data-activityid="${sa.activityId}"
                                    data-date="${dateStr}"
                                    data-sem="${sa.semester}"
                                    data-notes="${sa.notes || ""}"
                                >Редагувати</button>
                                <button class="btn btn-danger btn-sm delete-btn" data-id="${sa.id}">Видалити</button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження студентських активностей.");
            }
        });
    }

    function loadDropdownData() {
        $.ajax({
            url: "/StudentActivities/GetDropdownData",
            type: "GET",
            success: function (data) {
                fillSelect($studentSelect, data.students, "fullName");
                fillSelect($activitySelect, data.activities, "name");
            },
            error: function () {
                alert("Помилка завантаження списків.");
            }
        });
    }

    function fillSelect($sel, arr, textField) {
        $sel.empty();
        $sel.append(`<option value="">-- Обрати --</option>`);
        arr.forEach(item => {
            $sel.append(`<option value="${item.id}">${item[textField]}</option>`);
        });
    }

    function clearForm() {
        $saId.val("");
        $studentSelect.val("");
        $activitySelect.val("");
        $dateAwarded.val("");
        $semester.val("");
        $notes.val("");
    }

});
