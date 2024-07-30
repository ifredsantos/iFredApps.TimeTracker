CREATE DATABASE timetracker_app;

CREATE TABLE users (
    username VARCHAR(40) NOT NULL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NULL,
    password VARCHAR(100) NULL,
    created_at DATETIME NOT NULL DEFAULT NOW()
)

CREATE TABLE sessions (
    session_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    start_date DATETIME NOT NULL,
    end_date DATETIME NULL,
    description VARCHAR(255) NOT NULL,
    observation VARCHAR(500) NULL
)