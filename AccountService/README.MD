# Запуск
Для работы необходимо запустить keycloak и указать его расположение приложению через appsettings или env.
Приложение ожидает audience "account-service" и рассчитывает на Open id connect протокол.
Для запуска Keycloak можно использовать keycloak из docker-compose.yml: `docker-compose up -d keycloak`
Приложение можно запустить в таком случае, как локально(порт указан в launchsettings), так в Docker (порт 80).
Для запуска всей системы используйте `docker-compose up -d`

Swagger доступен по /swagger.

## Пользователи
В системе уже созданы следующие пользователи:
(uuid, username, password)
- `88bc045c-ecc8-4cb7-8cd9-b28938abef55`, testuser1, testpass1
- `6d8600da-cc20-42b9-a410-37c0b9e5b6c9`, testpass1, testpass2

Только они допускаются для создания новых счетов.
Регистрация новых пользователей возможна в keycloak в realm modulbank.


## Допустимые коды валют

В системе поддерживаются только следующие валюты:

- `RUB` — Российский рубль  
- `EUR` — Евро  
- `USD` — Доллар США

## Типы транзакций

- `Debit` — зачисление средств 
- `Credit` — списание средств

## Типы счетов

- `Checking` — расчетный счет  
- `Deposit` — депозитный счет  
- `Credit` — кредитный счет
