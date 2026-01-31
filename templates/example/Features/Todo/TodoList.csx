using System.Collections.Generic;
using CSX.Runtime;

component TodoList {
    var todos = UseState(new List<Todo> 
    { 
        new Todo { Text = "Learn CSX", Completed = true },
        new Todo { Text = "Build an App", Completed = false }
    });

    var newTodoText = UseState("");

    void AddTodo() {
        if (string.IsNullOrWhiteSpace(newTodoText.Value)) return;
        todos.Value.Add(new Todo { Text = newTodoText.Value, Completed = false });
        newTodoText.Value = ""; // Reset input
    }

    void Toggle(int index) {
        todos.Value[index].Completed = !todos.Value[index].Completed;
    }

    void Remove(int index) {
        todos.Value.RemoveAt(index);
    }

    return 
    <div>
        <div class="input-group">
            <input value={newTodoText.Value} oninput={e => newTodoText.Value = e.Value} placeholder="New Item..." />
            <button onclick={AddTodo}>Add</button>
        </div>
        <div class="list">
             <For each={todos.Value} item="todo" index="i">
                 <div class={Css.TodoItem}>
                     <input type="checkbox" checked={todo.Completed} onclick={() => Toggle(i)} />
                     <span class={todo.Completed ? Css.Completed : ""}>{todo.Text}</span>
                     <button class={Css.DeleteBtn} onclick={() => Remove(i)}>Delete</button>
                 </div>
             </For>
        </div>
    </div>;
}

public class Todo {
    public string Text { get; set; }
    public bool Completed { get; set; }
}
