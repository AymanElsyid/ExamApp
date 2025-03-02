document.addEventListener("DOMContentLoaded", function () {
    let questionIndex = 0;
    let examId = new URLSearchParams(window.location.search).get("ExamID");

    if (examId) {
        fetchExamData(examId);
    }

    async function fetchExamData(examId) {
        try {
            let response = await fetch(`/Admin/Exam/GetExam?Id=${examId}`);
            if (!response.ok) {
                throw new Error("Failed to fetch exam details.");
            }
            let data = await response.json();
            populateExamForm(data);
        } catch (error) {
            Swal.fire("Error!", error.message, "error");
        }
    }
    function populateExamForm(examData) {
        document.getElementById("examTitle").value = examData.examTitle;
        document.getElementById("showInHomePage").checked = examData.showInHomePage;

        examData.questions.forEach((question, index) => {
            addQuestion(question, index);
            
        });

        questionIndex = examData.questions.length;
    }



    document.getElementById("addQuestion").addEventListener("click", function () {
        let examTitle = document.getElementById("examTitle").value.trim();
        if (!examTitle) {
            Swal.fire('Warning!', 'Please enter the exam title.', 'warning');
            return;
        }
        if (!CheckQuestionValid()) return;
        addQuestion();


    });

    function addQuestion(questionData = null, existingIndex = null) {
        let index = existingIndex !== null ? existingIndex : questionIndex;
        let questionCard = document.createElement("div");
        questionCard.classList.add("card", "mt-3");
        questionCard.dataset.index = index;

        questionCard.innerHTML = `
            <div class="card-header">
                <h5 class="mb-0">Question ${index + 1}
                    <button type="button" class="btn btn-danger btn-sm float-right remove-question">Delete</button>
                </h5>
            </div>
            <div class="card-body">
                <label>Question Text:</label>
                <textarea class="form-control question-text">${questionData ? questionData.questionTitle : ""}</textarea>
                <div class="mt-3 answers-container" id="answersContainer${index}"></div>
                <button type="button" class="btn btn-secondary btn-sm mt-2 add-answer" data-question-index="${index}">Add Answer</button>
            </div>
        `;

        document.getElementById("questionsContainer").appendChild(questionCard);

        questionCard.querySelector(".remove-question").addEventListener("click", function () {
            questionCard.remove();
            reorderQuestions();
        });

        questionCard.querySelector(".add-answer").addEventListener("click", function () {
            let questionIdx = this.getAttribute("data-question-index");
            addAnswer(questionIdx);
        });

        if (questionData) {
            questionData.answers.forEach(answer =>
                addAnswer(index, answer.answerTitle, answer.isCorrect)
            );
        }

        questionIndex++;
    }

    function reorderQuestions() {
        let questions = document.querySelectorAll("#questionsContainer .card");
        questionIndex = 0;

        questions.forEach((questionCard, index) => {
            questionIndex = index; // Update global index

            questionCard.dataset.index = index;
            questionCard.querySelector(".card-header h5").innerHTML = `Question ${index + 1}
                <button type="button" class="btn btn-danger btn-sm float-right remove-question">Delete</button>`;

            let answersContainer = questionCard.querySelector(".answers-container");
            answersContainer.id = `answersContainer${index}`;

            let addAnswerBtn = questionCard.querySelector(".add-answer");
            addAnswerBtn.setAttribute("data-question-index", index);

            addAnswerBtn.removeEventListener("click", addAnswer);
            addAnswerBtn.addEventListener("click", function () {
                addAnswer(index);
            });

            let removeBtn = questionCard.querySelector(".remove-question");
            removeBtn.removeEventListener("click", reorderQuestions);
            removeBtn.addEventListener("click", function () {
                questionCard.remove();
                reorderQuestions();
            });
        });

        questionIndex++;
    }
    function CheckQuestionValid() {
        debugger;
        let lastQuestionCard = document.querySelector("#questionsContainer .card:last-child");
        if (lastQuestionCard) {
            let lastQuestionText = lastQuestionCard.querySelector(".question-text").value.trim();
            let answers = lastQuestionCard.querySelectorAll(".answer-text");
            let correctAnswers = lastQuestionCard.querySelectorAll(".correct-answer:checked");

            if (!lastQuestionText && questionIndex !== 0) {
                Swal.fire("Warning!", "Please enter the last question before adding a new one.", "warning");
                return false;
            }

            if (answers.length < 2) {
                Swal.fire("Warning!", "Each question must have at least two answers.", "warning");
                return false;
            }

            if (correctAnswers.length !== 1) {
                Swal.fire("Warning!", "Each question must have exactly one correct answer.", "warning");
                return false;
            }
        }

        return true;
    }

    function CheckValidAnswer(questionIdx) {
        let answersContainer = document.getElementById(`answersContainer${questionIdx}`);

        let answerCount = answersContainer.querySelectorAll(".answer-text").length;
        if (answerCount >= 4) {
            Swal.fire("Warning!", "Each question can have a maximum of 4 answers.", "warning");
            return false;
        }

        let answerDivs = answersContainer.querySelectorAll(".d-flex");
        let lastAnswerInput = answerDivs.length > 0 ? answerDivs[answerDivs.length - 1].querySelector(".answer-text") : null;

        if (lastAnswerInput && lastAnswerInput.value.trim() === "") {
            Swal.fire("Warning!", "Please enter the last answer before adding a new one.", "warning");
            return false;
        }

        return true;
    }
    function addAnswer(questionIdx, text = "", isCorrect = false) {
        let answersContainer = document.getElementById(`answersContainer${questionIdx}`);
        if (!answersContainer) return false;
        
        if (!CheckValidAnswer(questionIdx)) return;

        let answerDiv = document.createElement("div");
        answerDiv.classList.add("d-flex", "align-items-center", "mt-2");

        let input = document.createElement("input");
        input.type = "text";
        input.classList.add("form-control", "answer-text", "mx-4");
        input.value = text;

        let correctCheckbox = document.createElement("input");
        correctCheckbox.type = "checkbox";
        correctCheckbox.classList.add("correct-answer", "mx-2");
        correctCheckbox.checked = isCorrect;

        correctCheckbox.addEventListener("change", function () {
            let checkboxes = answersContainer.querySelectorAll(".correct-answer");
            checkboxes.forEach(cb => {
                if (cb !== correctCheckbox) cb.checked = false;
            });
        });

        let removeBtn = document.createElement("button");
        removeBtn.type = "button";
        removeBtn.classList.add("btn", "btn-danger", "btn-sm");
        removeBtn.textContent = "Remove";
        removeBtn.addEventListener("click", function () {
            answerDiv.remove();
        });

        answerDiv.appendChild(input);
        answerDiv.appendChild(correctCheckbox);
        answerDiv.appendChild(removeBtn);
        answersContainer.appendChild(answerDiv);
    }

    document.getElementById("examForm").addEventListener("submit", function (event) {
        event.preventDefault();
        debugger;
        if (!CheckQuestionValid()) return;
        if (!CheckValidAnswer(questionIndex - 1)) return;

        let examTitle = document.getElementById("examTitle").value.trim();
        if (!examTitle) {
            Swal.fire('Warning!', 'Please enter the exam title.', 'warning');
            return;
        }

        let questions = [];
        document.querySelectorAll("#questionsContainer .card").forEach((questionCard, index) => {
            let questionText = questionCard.querySelector(".question-text").value.trim();
            let answers = [];
            let correctAnswers = 0;

            questionCard.querySelectorAll(".answer-text").forEach((answerInput, idx) => {
                let isCorrect = questionCard.querySelectorAll(".correct-answer")[idx].checked;
                if (isCorrect) correctAnswers++;
                answers.push({ AnswerTitle: answerInput.value.trim(), IsCorrect: isCorrect });
            });

            if (!questionText || answers.length < 2) {
                Swal.fire('Warning!', 'Each question must have a title and at least two answers.', 'warning');
                return;
            }

            if (correctAnswers !== 1) {
                Swal.fire('Warning!', 'Each question must have exactly one correct answer.', 'warning');
                return;
            }

            questions.push({ QuestionTitle: questionText, Answers: answers });
        });

        if (questions.length === 0) {
            Swal.fire('Warning!', 'Please add at least one question.', 'warning');
            return;
        }

        let examData = {
            ExamID: examId || 0,
            ExamTitle: examTitle,
            ShowInHomePage: document.getElementById("showInHomePage").checked,
            Questions: questions
        };

        let url = examId ? `/Admin/Exam/EditExam` : `/Admin/Exam/CreateExam`;
        let method = examId ? "PUT" : "POST";

        fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(examData)
        })
            .then(() => {
                Swal.fire('Success!', 'Exam has been saved successfully!', 'success')
                    .then(() => window.location.href = "/Admin/Exam/Index");
            })
            .catch(() => Swal.fire('Error!', 'Something went wrong.', 'error'));
    });
});
