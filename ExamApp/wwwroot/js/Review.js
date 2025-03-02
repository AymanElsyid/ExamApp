document.addEventListener("DOMContentLoaded", function () {
    async function fetchExamData() {
        try {
            const examId = getExamIdFromUrl();
            if (!examId) {
                console.error("No examId found in URL");
                return;
            }
            let response = await fetch(`/exam/getReviewExam?Id=${examId}`);
            if (!response.ok) throw new Error("Failed to fetch exam details.");

            let data = await response.json();
            displayExamData(data);
        } catch (error) {
            Swal.fire("Error!", error.message, "error");
        }
    }

    function displayExamData(examData) {
        document.getElementById("Title1").textContent = examData.examTitle;
        const container = document.getElementById("questions-container");
        container.innerHTML = "";

        examData.questions.forEach((q, index) => {
            const section = document.createElement("section");
            section.classList.add("question-section", "mb-4", "p-3", "border", "rounded");

            section.innerHTML = `<h3>Question ${index + 1}</h3>
                                 <p><strong>${q.questionTitle}</strong></p>`;

            q.answers.forEach((answer) => {
                const div = document.createElement("div");
                div.classList.add("pt-2");

                div.innerHTML = `
                    <input class="form-check-input" type="radio"
                        name="${q.id}" 
                        id="answer-${q.id}-${answer.id}"  
                        value="${answer.id}" disabled>
                    <label class="form-check-label" for="answer-${q.id}-${answer.id}">
                        ${answer.answerTitle}
                    </label>`;

                section.appendChild(div);
            });

            container.appendChild(section);
        });

        
        markCorrectAnswers(examData.questions);
        markWrongAnswers(examData.uQuestions);
    }

    function markCorrectAnswers(questions) {
        questions.forEach((q) => {
            q.answers.forEach((answer) => {
                if (answer.isCorrect) {
                    let correctAnswerElement = document.querySelector(`#answer-${q.id}-${answer.id}`);
                    if (correctAnswerElement) {
                        correctAnswerElement.checked = true; 
                        correctAnswerElement.nextElementSibling.style.color = "green"; 
                    }
                }
            });
        });
    }

    function markWrongAnswers(uQuestions) {
        uQuestions.forEach((uq) => {
            if (uq.answerId !== uq.correctAnswerId) {
                let wrongAnswerElement = document.querySelector(`#answer-${uq.questionId}-${uq.answerId}`);
                if (wrongAnswerElement) {
                    wrongAnswerElement.checked = true; 
                    let label = wrongAnswerElement.nextElementSibling;
                    label.style.color = "red"; 
                    label.style.textDecoration = "line-through"; 
                }
            }
        });
    }

    function getExamIdFromUrl() {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get("Id");
    }

    fetchExamData();
});
