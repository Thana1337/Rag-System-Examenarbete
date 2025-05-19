import Sidebar from './components/Sidebar';
import Chat from './components/Chat';

export default function App() {
  return (
    <div className="h-full flex">
      <Sidebar />
      <Chat />
    </div>
  );
}