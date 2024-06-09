# Pizarra Distribuida

Este proyecto demuestra una pizarra distribuida utilizando sockets en C#. Incluye una aplicación de servidor y una aplicación de cliente que pueden ejecutarse en diferentes dispositivos.

### Instrucciones

1. Descargar el repositorio en tu máquina local.
2. Inicia la aplicación del servidor. Abre una terminal y navega hasta el directorio `serverApp`, luego ejecuta el comando:
    ```bash
    dotnet run
    ```

3. Inicia la aplicación del cliente en otros dispositivos. Abre una terminal en cada dispositivo cliente, navega hasta el directorio `clientApp` y ejecuta el comando:
    ```bash
    dotnet run
    ```

### Nota

Asegúrate de cambiar la dirección IP en los clientes para que apunten a la IP del servidor. Puedes hacer esto modificando el archivo de configuración en `clientApp`.

### Demostración en Video

Para ver una demostración en video de cómo funciona esta pizarra distribuida, visita el siguiente enlace:
- 🎥 [Ver en YouTube](https://www.youtube.com/watch?v=-WBaBFFRW2w)

### Estructura del Proyecto

- `serverApp/`: Contiene la aplicación del servidor.
- `clientApp/`: Contiene la aplicación del cliente.

### Requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado en los dispositivos cliente y servidor.

---

¡Gracias por usar nuestra Pizarra Distribuida!

