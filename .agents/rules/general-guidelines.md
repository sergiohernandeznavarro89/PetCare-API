# Instrucciones Generales para la Inteligencia Artificial (Antigravity)

Este archivo contiene las directrices, reglas de estilo y buenas prácticas que debes seguir estrictamente cuando trabajes en este workspace. 
*Nota: Actualmente se encuentra en la carpeta del backend. Cuando se añada el frontend, puede moverse a la carpeta raíz del proyecto para que aplique a ambos de forma global.*

## 1. Buenas Prácticas de Código
- **Comentarios Explicativos:** Añade siempre comentarios descriptivos y en español en la cabecera de las clases importantes, controladores y métodos públicos. Explica el *por qué* y la intención de la lógica, no solo el *qué*.
- **Arquitectura Limpia:** Mantiene la separación de responsabilidades. Para el backend usamos arquitectura de Vertical Slices con **CQRS (MediatR)**. Los controladores deben ser muy finos y limitarse a recibir la petición HTTP y despachar el comando/query a la feature correspondiente.
- **Nomenclatura (Clean Code):** Utiliza nombres de variables, métodos y clases que revelen la intención. Evita nombres genéricos como `data`, `info`, o `manager`.
- **Manejo de Errores:** Nunca tragues excepciones. Devuelve siempre las respuestas HTTP adecuadas (404 NotFound, 401 Unauthorized, 403 Forbidden, 400 BadRequest) según la situación, con mensajes de error claros.
- **Tests Unitarios Obligatorios:** Todos los endpoints y controladores nuevos que se creen deben venir acompañados inmediatamente de sus respectivos tests unitarios en el proyecto de pruebas, asegurando que devuelven los códigos HTTP correctos y simulan (Mock) correctamente las dependencias (como MediatR).
- **Mantenimiento del Esquema de Base de Datos:** Cada vez que se cree una nueva migración de Entity Framework Core para modificar la base de datos, es OBLIGATORIO generar el script SQL del esquema y sobreescribir el archivo `schema.sql` en la raíz del proyecto backend (usando `dotnet ef migrations script -o schema.sql`).

## 2. Pautas de Interacción y Gestión del Repositorio
- **Control de Versiones (Git):** 
  - **REGLA DE ORO:** NUNCA ejecutes un comando `git commit`, `git push` ni generes una Pull Request (PR) a menos que el usuario (yo) te lo pida de forma **explícita y directa**. 
  - Limítate a escribir el código, compilar, hacer tests y probar, dejando que sea yo quien gestione, revise y suba los cambios finales al control de versiones.
- **Cambios Pequeños e Iterativos:** Cuando te pida desarrollar una feature, hazlo paso a paso para asegurar que todo compila y funciona, en lugar de generar archivos gigantescos de una sola vez.

## 3. Consideraciones para el Frontend (Futuro)
- En el frontend deberás seguir una arquitectura basada en componentes reutilizables.
- Toda comunicación con la API deberá gestionar correctamente el paso del **Token JWT (Bearer)** y gestionar el estado `401 Unauthorized` para redirigir al Login en caso de que caduque la sesión.
- Prioriza diseños modernos, muy visuales e interacciones fluidas que den sensación "Premium" (Responsive, animaciones sutiles, paletas cuidadas).
