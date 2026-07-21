const messageElement =
    document.querySelector("#message");

const reloadButton =
    document.querySelector("#reloadButton");

const createTicketButton =
    document.querySelector("#createTicketButton");

const createTicketDialog =
    document.querySelector("#createTicketDialog");

const closeTicketDialogButton =
    document.querySelector("#closeTicketDialogButton");

const cancelCreateTicketButton =
    document.querySelector("#cancelCreateTicketButton");

const createTicketForm =
    document.querySelector("#createTicketForm");

const prioritySelect =
    document.querySelector("#ticketPriority");

const createTicketError =
    document.querySelector("#createTicketError");

const submitTicketButton =
    document.querySelector("#submitTicketButton");

const ticketTitleInput =
    document.querySelector("#ticketTitle");

const ticketCreatedByInput =
    document.querySelector("#ticketCreatedBy");


let currentTickets = [];


/*
 * Event-Listener
 */

reloadButton.addEventListener(
    "click",
    loadTickets
);

createTicketButton.addEventListener(
    "click",
    openCreateTicketDialog
);

closeTicketDialogButton.addEventListener(
    "click",
    closeCreateTicketDialog
);

cancelCreateTicketButton.addEventListener(
    "click",
    closeCreateTicketDialog
);

createTicketForm.addEventListener(
    "submit",
    createTicket
);

/*
 * Dialog schließen, wenn neben den eigentlichen
 * Dialoginhalt geklickt wird.
 */
createTicketDialog.addEventListener(
    "click",
    event => {
        if (event.target === createTicketDialog) {
            closeCreateTicketDialog();
        }
    }
);


setupDropZones();
loadTickets();


/*
 * Ticket-Erstellung
 */

async function openCreateTicketDialog() {
    createTicketError.textContent = "";

    await loadPriorities();

    createTicketDialog.showModal();
    ticketTitleInput.focus();
}


function closeCreateTicketDialog() {
    createTicketDialog.close();
}


async function loadPriorities() {
    /*
     * Wenn bereits Optionen geladen wurden,
     * müssen sie nicht erneut geladen werden.
     */
    if (prioritySelect.options.length > 1) {
        return;
    }

    try {
        const response = await fetch(
            "/tickets/priorities"
        );

        if (!response.ok) {
            throw new Error(
                `HTTP-Status ${response.status}`
            );
        }

        const priorities = await response.json();

        for (const priority of priorities) {
            const option =
                document.createElement("option");

            option.value = priority.id;
            option.textContent = priority.name;

            prioritySelect.append(option);
        }
    } catch (error) {
        console.error(error);

        createTicketError.textContent =
            "Prioritäten konnten nicht geladen werden.";
    }
}


async function createTicket(event) {
    event.preventDefault();

    createTicketError.textContent = "";

    submitTicketButton.disabled = true;
    submitTicketButton.textContent =
        "Wird erstellt …";

    const formData =
        new FormData(createTicketForm);

    const requestBody = {
        title:
            formData.get("title").trim(),

        description:
            formData.get("description").trim(),

        priorityId:
            Number(formData.get("priorityId")),

        createdBy:
            formData.get("createdBy").trim()
    };

    try {
        const response = await fetch(
            "/tickets",
            {
                method: "POST",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify(requestBody)
            }
        );

        if (!response.ok) {
            let errorMessage =
                `HTTP-Status ${response.status}`;

            try {
                const errorBody =
                    await response.json();

                if (errorBody.message) {
                    errorMessage =
                        errorBody.message;
                }
            } catch {
                // Die Antwort enthielt kein JSON.
            }

            throw new Error(errorMessage);
        }

        const createdTicket =
            await response.json();

        createTicketDialog.close();
        createTicketForm.reset();

        /*
         * Testbenutzer wieder einsetzen.
         */
        ticketCreatedByInput.value = "Semih";

        await loadTickets();

        messageElement.textContent =
            `Ticket #${createdTicket.id} wurde erstellt.`;
    } catch (error) {
        console.error(error);

        createTicketError.textContent =
            error.message;
    } finally {
        submitTicketButton.disabled = false;

        submitTicketButton.textContent =
            "Ticket erstellen";
    }
}


/*
 * Tickets laden
 */

async function loadTickets() {
    messageElement.textContent =
        "Tickets werden geladen …";

    clearBoard();

    try {
        const response =
            await fetch("/tickets");

        if (!response.ok) {
            throw new Error(
                `API antwortet mit Status ${response.status}`
            );
        }

        currentTickets =
            await response.json();

        updateCounters(currentTickets);
        renderTickets(currentTickets);

        messageElement.textContent =
            `${currentTickets.length} Tickets geladen.`;
    } catch (error) {
        console.error(error);

        messageElement.textContent =
            "Tickets konnten nicht geladen werden.";
    }
}


function clearBoard() {
    const ticketLists =
        document.querySelectorAll(".ticket-list");

    for (const ticketList of ticketLists) {
        ticketList.replaceChildren();
    }
}


/*
 * Statistik
 */

function updateCounters(tickets) {
    const totalCount =
        tickets.length;

    const openCount =
        tickets.filter(
            ticket => ticket.statusId === 1
        ).length;

    const progressCount =
        tickets.filter(
            ticket => ticket.statusId === 2
        ).length;

    /*
     * Resolved und Closed zählen beide als erledigt.
     */
    const resolvedCount =
        tickets.filter(
            ticket =>
                ticket.statusId === 3 ||
                ticket.statusId === 4
        ).length;

    document.querySelector(
        "#totalCount"
    ).textContent = totalCount;

    document.querySelector(
        "#openCount"
    ).textContent = openCount;

    document.querySelector(
        "#progressCount"
    ).textContent = progressCount;

    document.querySelector(
        "#resolvedCount"
    ).textContent = resolvedCount;
}


/*
 * Tickets darstellen
 */

function renderTickets(tickets) {
    for (const ticket of tickets) {
        const ticketList =
            document.querySelector(
                `#status-${ticket.statusId}`
            );

        if (!ticketList) {
            continue;
        }

        const card =
            createTicketCard(ticket);

        ticketList.append(card);
    }
}


function createTicketCard(ticket) {
    const card =
        document.createElement("article");

    card.className = "ticket-card";
    card.draggable = true;
    card.dataset.ticketId = ticket.id;


    const title =
        document.createElement("h3");

    title.textContent =
        `#${ticket.id} ${ticket.title}`;


    const description =
        document.createElement("p");

    description.textContent =
        ticket.description;


    const metadata =
        document.createElement("small");

    const priorityName =
        ticket.priority?.name ??
        `Priorität ${ticket.priorityId}`;

    metadata.textContent =
        `${priorityName} · Erstellt von ${ticket.createdBy}`;


    card.append(
        title,
        description,
        metadata
    );


    card.addEventListener(
        "dragstart",
        event => {
            card.classList.add("dragging");

            event.dataTransfer.setData(
                "text/plain",
                ticket.id.toString()
            );

            event.dataTransfer.effectAllowed =
                "move";
        }
    );


    card.addEventListener(
        "dragend",
        () => {
            card.classList.remove("dragging");

            const columns =
                document.querySelectorAll(
                    ".column"
                );

            for (const column of columns) {
                column.classList.remove(
                    "drag-over"
                );
            }
        }
    );

    card.addEventListener("click", event => {
    /*
     * Beim Ziehen soll nicht gleichzeitig
     * das Detailfenster geöffnet werden.
     */
    if (card.classList.contains("dragging")) {
        return;
    }

    openTicketDetails(ticket);
});


    return card;
}


/*
 * Drag-and-drop
 */

function setupDropZones() {
    const columns =
        document.querySelectorAll(".column");

    for (const column of columns) {
        column.addEventListener(
            "dragover",
            event => {
                event.preventDefault();

                event.dataTransfer.dropEffect =
                    "move";

                column.classList.add(
                    "drag-over"
                );
            }
        );


        column.addEventListener(
            "dragleave",
            event => {
                if (
                    !column.contains(
                        event.relatedTarget
                    )
                ) {
                    column.classList.remove(
                        "drag-over"
                    );
                }
            }
        );


        column.addEventListener(
            "drop",
            async event => {
                event.preventDefault();

                column.classList.remove(
                    "drag-over"
                );

                const ticketId =
                    Number(
                        event.dataTransfer.getData(
                            "text/plain"
                        )
                    );

                const newStatusId =
                    Number(
                        column.dataset.statusId
                    );

                await moveTicket(
                    ticketId,
                    newStatusId,
                    column
                );
            }
        );
    }
}


async function moveTicket(
    ticketId,
    newStatusId,
    targetColumn
) {
    const ticket =
        currentTickets.find(
            currentTicket =>
                currentTicket.id === ticketId
        );

    if (!ticket) {
        messageElement.textContent =
            `Ticket #${ticketId} wurde nicht gefunden.`;

        return;
    }

    if (ticket.statusId === newStatusId) {
        messageElement.textContent =
            `Ticket #${ticketId} befindet sich bereits in dieser Spalte.`;

        return;
    }

    const oldStatusId =
        ticket.statusId;

    const card =
        document.querySelector(
            `[data-ticket-id="${ticketId}"]`
        );

    const targetList =
        targetColumn.querySelector(
            ".ticket-list"
        );

    /*
     * Karte wird sofort auf der Oberfläche verschoben.
     */
    ticket.statusId = newStatusId;

    if (card && targetList) {
        targetList.append(card);
    }

    updateCounters(currentTickets);

    messageElement.textContent =
        `Ticket #${ticketId} wird gespeichert …`;

    try {
        const response = await fetch(
            `/tickets/${ticketId}`,
            {
                method: "PUT",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify({
                    statusId: newStatusId
                })
            }
        );

        if (!response.ok) {
            const errorText =
                await response.text();

            throw new Error(
                `HTTP ${response.status}: ${errorText}`
            );
        }

        const updatedTicket =
            await response.json();

        /*
         * Lokales Ticket mit der API-Antwort aktualisieren.
         */
        Object.assign(
            ticket,
            updatedTicket
        );

        messageElement.textContent =
            `Ticket #${ticketId} wurde gespeichert.`;
    } catch (error) {
        console.error(error);

        ticket.statusId =
            oldStatusId;

        messageElement.textContent =
            `Ticket #${ticketId} konnte nicht gespeichert werden.`;

        /*
         * Bei einem Fehler wird der echte Zustand
         * erneut aus PostgreSQL geladen.
         */
        await loadTickets();
    }
}