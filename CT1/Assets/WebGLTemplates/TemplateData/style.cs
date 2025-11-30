body {
    padding: 0;
margin: 0;
background: linear - gradient(135deg, #667eea 0%, #764ba2 100%);
    display: flex;
justify - content: center;
align - items: center;
height: 100vh;
font - family: 'Arial', sans - serif;
}

.webgl - content {
position: relative;
    text - align: center;
}

# unity-container {
background: #000;
    border - radius: 10px;
box - shadow: 0 10px 30px rgba(0,0,0,0.3);
}

.loading - bar {
position: absolute;
left: 50 %;
top: 50 %;
transform: translate(-50 %, -50 %);
height: 5px;
width: 200px;
background: rgba(255, 255, 255, 0.2);
    border - radius: 3px;
overflow: hidden;
}

.loading - bar:before {
    content: '';
position: absolute;
height: 100 %;
width: 100 %;
background: linear - gradient(90deg, #ff6b6b, #feca57);
    border - radius: 3px;
animation: loading 2s infinite;
}

@keyframes loading
{
    0% { transform: translateX(-100 %); }
    100% { transform: translateX(100 %); }
}

.footer {
    margin-top: 20px;
display: flex;
justify - content: center;
align - items: center;
gap: 20px;
}

.title {
    color: white;
font - size: 24px;
font - weight: bold;
text - shadow: 2px 2px 4px rgba(0,0,0,0.5);
}

.fullscreen {
    width: 38px;
height: 38px;
background: url('progress-bar.png') no - repeat center;
background - size: contain;
cursor: pointer;
transition: transform 0.2s;
}

.fullscreen: hover {
transform: scale(1.1);
}