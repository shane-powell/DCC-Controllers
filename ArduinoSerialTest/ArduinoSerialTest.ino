char c;

void setup() {
  // put your setup code here, to run once:
  //Serial.begin(115200);
  Serial.begin(9600);
  Serial.flush();
}

void loop() {
  // put your main code here, to run repeatedly:
  while (Serial.available() > 0)
  {
    c = Serial.read();
    Serial.write(c);
  }

}
