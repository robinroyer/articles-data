
# Setting up the database

CREATE DATABASE Pagination;
CREATE TABLE objects (
    id INT NOT NULL AUTO_INCREMENT,
    content VARCHAR(255) NOT NULL,
    tenant VARCHAR(255) NOT NULL,
    creationDate DATETIME NOT NULL,
    CONSTRAINT objects_pk PRIMARY KEY (id)
);
CREATE INDEX pagination_idx on objects(tenant, creationDate);

DELIMITER //
FOR i in 1..1000000
DO
    set @num = CEILING(RAND()*4);
    set @content = LEFT(MD5(RAND()), 30);
    set @tenant = ELT(@num, 'tenant-1', 'tenant-2', 'tenant-3', 'tenant-4');
    set @date = DATE_ADD(UTC_DATE(), INTERVAL i DAY);
    insert into objects values(
        i,
        @content,
        @tenant,
        @date);
END FOR;
//
DELIMITER ;


