create database exam_acceloka;

create table Users (
    user_id SERIAL primary key,
    username VARCHAR(50) unique not null,
    email VARCHAR(100) unique not null,
    password_hash TEXT not null,
    created_at TIMESTAMP default CURRENT_TIMESTAMP
);


create table Category (
    category_id SERIAL primary key,
    category_name VARCHAR(100) unique not null
);

create table Ticket (
    ticket_code VARCHAR(10) primary key not null,
    ticket_name VARCHAR(255) not null,
    category_id INT not null,
    event_date TIMESTAMP not null,
    price INT not null,
    quota INT not null,
    foreign key (category_id) references Category(category_id) on delete cascade
);

create table BookedTicket (
    booked_ticket_id SERIAL primary key,
    user_id INT not null,
    booked_at TIMESTAMP default CURRENT_TIMESTAMP,
    foreign key (user_id) references users(user_id) on delete cascade
);

create table BookedTicketDetail (
    booked_ticket_detail_id SERIAL primary key,
    booked_ticket_id INT not null,
    ticket_code VARCHAR(10) not null,
    quantity INT not null,
    foreign key (booked_ticket_id) references BookedTicket(booked_ticket_id) on delete cascade,
    foreign key (ticket_code) references Ticket(ticket_code) on delete cascade
);

insert into category (category_name) values 
('Transportasi Darat'),
('Transportasi Laut'),
('Cinema'),
('Hotel'),
('Tranportasi Terbang'),
('Konser');

insert into ticket (ticket_code, ticket_name, category_id, quota, event_date, price) values
('TD001', 'Bus jawa-sumatra', 1, 80, '2025-03-01 17:59:00', 5000000),
('TL001', 'Kapal Ferri jawa-sumatra', 2, 70, '2025-03-01 17:59:00', 5000000), 
('C001', 'Ironman CGV', 3, 99, '2025-03-02 17:59:00', 5000000), 
('C002', 'Black Panther', 3, 80, '2025-03-02 17:59:00', 5000000),
('H001', 'Ibis Hotel Jakarta 21-23', 4, 76, '2025-03-01 17:59:00', 5000000);

insert into users (username, email, password_hash) values
('Admin', 'admin001@gmail.com', 'admin123');

