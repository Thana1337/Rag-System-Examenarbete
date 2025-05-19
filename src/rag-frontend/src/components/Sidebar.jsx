export default function Sidebar() {
  return (
    <aside className="w-64 bg-primary/20 backdrop-blur-lg text-white p-6 flex flex-col">
      <h1 className="text-2xl font-extrabold mb-6">RAG System</h1>
      <nav className="flex-1">
        <ul className="space-y-5">
          <li><a href="#" className="hover:text-primary">New Chat</a></li>
          <li><a href="#" className="hover:text-primary">History</a></li>
          <li><a href="#" className="hover:text-primary">Settings</a></li>
        </ul>
      </nav>
    </aside>
  );
}