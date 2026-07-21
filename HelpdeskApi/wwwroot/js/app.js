const messageElement = document.querySelector("#message");
const reloadButton = document.querySelector("#reloadButton");

let currentTickets = [];

reloadButton.addEventListener("click", loadTickets);

setupDropZones();
loadTickets();


async function loadTickets() {
    messageElement.textContent = "Tickets werden geladen …";

    clearBoard();

    try {
        const response = await fetch("/tickets");

        if (!response.ok) {
            throw new Error(
                `API antwortet mit Status ${response.status}`
            );
        }

        currentTickets = await response.json();

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


function updateCounters(tickets) {
    document.querySelector("#totalCount").textContent =
        tickets.length;

    document.querySelector("#openCount").textContent =
        tickets.filter(ticket => ticket.statusId === 1).length;

    document.querySelector("#progressCount").textContent =
        tickets.filter(ticket => ticket.statusId === 2).length;

    document.querySelector("#resolvedCount").textContent =
        tickets.filter(
            ticket =>
                ticket.statusId === 3 ||
                ticket.statusId === 4
        ).length;
}

function renderTickets(tickets) {
    for (const ticket of tickets) {
        const column = document.querySelector(
            `#status-${ticket.statusId}`
        );

        if (!column) {
            continue;
        }

        const card = createTicketCard(ticket);

        column.append(card);
    }
}


function createTicketCard(ticket) {
    const card = document.createElement("article");

    card.className = "ticket-card";

    // Macht die Karte mit der Maus verschiebbar.
    card.draggable = true;

    // Speichert die Ticket-ID direkt am HTML-Element.
    card.dataset.ticketId = ticket.id;


    const title = document.createElement("h3");

    title.textContent =
        `#${ticket.id} ${ticket.title}`;


    const description = document.createElement("p");

    description.textContent = ticket.description;


    const metadata = document.createElement("small");

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


    card.addEventListener("dragstart", event => {
        card.classList.add("dragging");

        event.dataTransfer.setData(
            "text/plain",
            ticket.id.toString()
        );

        event.dataTransfer.effectAllowed = "move";
    });


    card.addEventListener("dragend", () => {
        card.classList.remove("dragging");
    });


    return card;
}


function setupDropZones() {
    const columns = document.querySelectorAll(".column");

    for (const column of columns) {
        column.addEventListener("dragover", event => {
            event.preventDefault();

            event.dataTransfer.dropEffect = "move";
            column.classList.add("drag-over");
        });

        column.addEventListener("dragleave", event => {
            if (!column.contains(event.relatedTarget)) {
                column.classList.remove("drag-over");
            }
        });

        column.addEventListener("drop", async event => {
            event.preventDefault();
            column.classList.remove("drag-over");

            const ticketId = Number(
                event.dataTransfer.getData("text/plain")
            );

            const newStatusId = Number(
                column.dataset.statusId
            );

            await moveTicket(ticketId, newStatusId, column);
        });
    }
}


async function moveTicket(
    ticketId,
    newStatusId,
    targetColumn
) {
    const ticket = currentTickets.find(
        currentTicket => currentTicket.id === ticketId
    );

    if (!ticket) {
        messageElement.textContent =
            `Ticket #${ticketId} wurde nicht gefunden.`;

        return;
    }

    if (ticket.statusId === newStatusId) {
        return;
    }

    const oldStatusId = ticket.statusId;

    const card = document.querySelector(
        `[data-ticket-id="${ticketId}"]`
    );

    const targetList =
        targetColumn.querySelector(".ticket-list");

    /*
     * Oberfläche sofort verändern.
     * Dadurch fühlt sich das Verschieben direkt an.
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
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    statusId: newStatusId
                })
            }
        );

        if (!response.ok) {
            const errorText = await response.text();

            throw new Error(
                `HTTP ${response.status}: ${errorText}`
            );
        }

        const updatedTicket = await response.json();

        /*
         * Lokale Daten mit der Antwort der API aktualisieren.
         */
        Object.assign(ticket, updatedTicket);

        messageElement.textContent =
            `Ticket #${ticketId} wurde gespeichert.`;
    } catch (error) {
        console.error(error);

        /*
         * Bei einem Fehler wieder den echten Zustand
         * aus PostgreSQL laden.
         */
        ticket.statusId = oldStatusId;

        messageElement.textContent =
            `Ticket #${ticketId} konnte nicht gespeichert werden.`;

        await loadTickets();
    }
}