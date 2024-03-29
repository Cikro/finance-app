CREATE TABLE IF NOT EXISTS transaction_types 
(
    id tinyint unsigned PRIMARY KEY AUTO_INCREMENT,
    name varchar(50) NOT NULL,
    date_created TIMESTAMP NOT NULL default CURRENT_TIMESTAMP,
    date_last_edited TIMESTAMP on update CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS transactions 
(
    id int unsigned PRIMARY KEY AUTO_INCREMENT,
    user_id int unsigned NOT NULL,
    account_id int unsigned NOT NULL,
    type tinyint unsigned NOT NULL,
    amount DECIMAL(15,2) NOT NULL,
    transaction_date date NOT NULL, # See https://mariadb.com/kb/en/library/date-and-time-literals/
    notes varchar(100) default NULL,
    journal_entry_id int unsigned NOT NULL,
    corrected boolean default false,
    server_generated boolean default false,
    date_created TIMESTAMP NOT NULL default CURRENT_TIMESTAMP,
    date_last_edited TIMESTAMP on update CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE CASCADE ON UPDATE RESTRICT,
    FOREIGN KEY (account_id) REFERENCES accounts(id)
        ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (type) REFERENCES transaction_types(id)
        ON DELETE RESTRICT ON UPDATE RESTRICT,
    FOREIGN KEY (journal_entry_id) REFERENCES journal_entries(id)
        ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Change the EOL delimiter to | so that the below trigger between BEGIN...END is considered
to be in the same statment */
DELIMITER |
/* Check for valid transaction_date */
CREATE TRIGGER IF NOT EXISTS validateTransactionDate
    BEFORE INSERT ON transactions
    FOR EACH ROW
    BEGIN
        IF NEW.transaction_date NOT LIKE '____-__-__' THEN
          SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'transaction_date must be in the form YYYY-MM-DD.';
        
        END If;
    END |
DELIMITER ;
