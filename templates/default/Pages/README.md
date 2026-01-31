# Pages Layer

In Feature-Sliced Design, the `Pages` layer is responsible for composing **Widgets**, **Features**, and **Entities** into full pages.

## Usage
- **Routing**: Files in this directory automatically become routes (e.g., `Home.csx` -> `/home`).
- **Composition**: Pages should have minimal logic. They should primarily import components from other layers and arrange them.

## Example
```csharp
using MyProject.Features.Auth;
using MyProject.Widgets.Layout;

component LoginPage {
   return <MainLayout>
       <LoginForm />
   </MainLayout>;
}
```
